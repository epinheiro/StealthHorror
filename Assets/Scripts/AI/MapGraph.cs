using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGraph
{
    float debugDelay = 5f;

    List<Room> rooms;

    Dictionary<int, MapGraphNode> nodes;

    HashSet<MapGraph> graph;

    // Both rooms and mapReferences obeys same indexes, as one was made from the other
    public MapGraph(List<Room> rooms)
    {
        this.rooms = rooms;

        nodes = new Dictionary<int, MapGraphNode>();
        foreach(Room room in rooms)
        {
            MapReference mapRef = room.MapReference;

            /// Inner paths
            List<MapGraphEdge> edges = mapRef.GetEdges();
            if(GameManager.Instance.DebugSettings.ShowContructingRoomGraph) Debug.LogError($"MapGraph Room {room.name} - edges [{edges.Count}] - adjacents rooms [{room.nodeToAdjacentDict.Count}]");
            foreach(MapGraphEdge edge in edges)
            {
                ProcessNode(edge.PointA, room, edge.PointB, room);
            }

            /// Inter paths
            foreach(KeyValuePair<Room, GameObject> pair in room.nodeToAdjacentDict)
            {
                GameObject point1 = room.GetTransitionNode(pair.Key);
                GameObject point2 = pair.Key.GetTransitionNode(room);
                ProcessNode(point1, room, point2, room.GetTransitionToRoom(point1));
                if(GameManager.Instance.DebugSettings.ShowContructingRoomGraph)
                {
                    Debug.DrawLine(point1.transform.position, point2.transform.position, Color.magenta, debugDelay);
                    GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = pair.Value.transform.position;
                }
            }
        }
    }

    private void ProcessNode(GameObject point1, Room room1, GameObject point2, Room room2)
    {
        int point1ID = point1.gameObject.GetInstanceID();
        if ( !nodes.ContainsKey(point1ID) )
        {
            if(GameManager.Instance.DebugSettings.ShowContructingRoomGraph) Debug.Log($"MapGraph ProcessNode {point1ID} - NEW [{point1.transform.parent.parent.name}] {point1.gameObject.name}");
            MapGraphNode newNode = new MapGraphNode(point1, room1);
            nodes.Add(point1ID, newNode);

            InsertNeighbors(newNode, point1, room1, point2, room2);
        }
        else
        {
            MapGraphNode node = nodes[point1ID];
            if ( !node.ContainsAdjacent(point2) )
                InsertNeighbors(node, point1, room1, point2, room2);
        }
    }

    void InsertNeighbors(MapGraphNode node, GameObject point1, Room room1, GameObject point2, Room room2)
    {
        int point1ID = point1.gameObject.GetInstanceID();
        if(GameManager.Instance.DebugSettings.ShowContructingRoomGraph) Debug.Log($"MapGraph ProcessNode {point1ID} - NEIGHBOR [{point1.transform.parent.parent.name}] {point1.gameObject.name} -> {point2.gameObject.GetInstanceID()} [{point2.transform.parent.parent.name}] {point2.gameObject.name}");
        node.InsertNeighbors(point2);
        ProcessNode(point2, room2, point1, room1);
    }

    public MapGraphNode GetNodeInfo(GraphPathData goNode)
    {
        MapGraphNode graphNode;
        nodes.TryGetValue(goNode.NodeInstanceID, out graphNode);
        return graphNode;
    }

    private List<GraphPathData> GetPath(GameObject agent, Vector3 toPosition, bool minimumCost)
    {
        GameObject fromNode = GetClosestNode(agent.transform.position);
        GameObject toNode = GetClosestNode(toPosition);

        List<GraphPathData> path = GetPath(fromNode, toNode, minimumCost);
        path.Insert(0, new GraphPathData(fromNode)); // Guarantee the first node

        return path;
    }

    public List<GraphPathData> GetMinimumPath(GameObject agent, Vector3 toPosition)
    {
        return GetPath(agent, toPosition, true);
    }

    public List<GraphPathData> GetRandomPath(GameObject agent)
    {
        return GetPath(agent, GetRandomNode().Vertex.transform.position, false);
    }

    public List<GraphPathData> GetRandomPath(GameObject agent, Vector3 toPosition)
    {
        return GetPath(agent, toPosition, false);
    }

    MapGraphNode GetRandomNode()
    {
        return nodes.ElementAt(UnityEngine.Random.Range(0, nodes.Keys.Count)).Value;
    }

    private List<GraphPathData> GetPath(GameObject from, GameObject to, bool minimumCost)
    {
        List<GraphPathData> path;

        if( minimumCost )
        {
            path = GetPathAStar(from, to);
        }
        else
        {
            GetPathRandomDepthRecursive(from, to, out path);
        }

        if(GameManager.Instance.DebugSettings.ShowPathFindingGraphs)
        {
            Color pathColor = minimumCost ? Color.red : Color.cyan;

            Debug.DrawLine(from.transform.position, to.transform.position, Color.blue, debugDelay+1);
            if (path != null)
            {
                string pathMsg = $"SEARCH RESULT {from.name} | ";
                if(path.Count > 0) Debug.DrawLine(from.transform.position, path[0].position, pathColor, debugDelay+1);
                for (int i = 0; i < path.Count-1; i++)
                {
                    pathMsg += $"{path[i].Name} | ";
                    Debug.DrawLine(path[i].position, path[i + 1].position, pathColor, debugDelay+1);
                }
                pathMsg += $"{path[path.Count-1].Name}";
                Debug.LogAssertion(pathMsg);
            }
        }

        return path;
    }

    private GameObject GetClosestNode(Vector3 checkPosition)
    {
        float minimumDistance = float.MaxValue;
        GameObject go = null;
        foreach(KeyValuePair<int, MapGraphNode> pair in nodes)
        {
            GameObject vertex = pair.Value.Vertex;
            float checkDistance = Vector2.Distance(vertex.transform.position, checkPosition);
            if(minimumDistance > checkDistance)
            {
                go = vertex;
                minimumDistance = checkDistance;
            }
        }
        return go;
    }

    private bool GetPathRandomDepthRecursive(GameObject from, GameObject to, out List<GraphPathData> path, int depth = 0, HashSet<int> visited = null)
    {
        int fromInstanceID = from.GetInstanceID();
        int toInstanceID = to.GetInstanceID();

        if(fromInstanceID == toInstanceID)
        {
            if(GameManager.Instance.DebugSettings.ShowPathFindingGraphs) Debug.LogError($"SEARCH - FOUND in depth {depth}");
            path = new List<GraphPathData>();
            path.Add(new GraphPathData(to));
            return true;
        }

        if(visited == null)
            visited = new HashSet<int>();
  
        if( !visited.Contains(fromInstanceID) )
            visited.Add(fromInstanceID);
        else
        {
            if(GameManager.Instance.DebugSettings.ShowPathFindingGraphs) Debug.LogAssertion($"SEARCH - already visited {from.name}");
            path = null;
            return false;
        }

        MapGraphNode graphNode = nodes[fromInstanceID];
        foreach(GameObject neighbor in graphNode.ShuffledNeighbors)
        {
            if(GameManager.Instance.DebugSettings.ShowPathFindingGraphs)
            {
                Debug.Log($"SEARCH - go {from.name} [{from.gameObject.GetInstanceID()}] to {neighbor.name} [{neighbor.gameObject.GetInstanceID()}]");
                Debug.DrawLine(from.transform.position, neighbor.transform.position, Color.black, debugDelay);
            }
            MapGraphNode neighborGraphNode = nodes[neighbor.GetInstanceID()];

            if( from.gameObject.GetInstanceID() != neighbor.gameObject.GetInstanceID() )
            {
                if(GetPathRandomDepthRecursive(neighbor, to, out path, depth++, visited))
                {
                    path.Insert(0, new GraphPathData(neighbor));
                    return true;
                }
            }
        }

        path = null;
        return false;
    }

    // Adapted from https://www.raywenderlich.com/3016-introduction-to-a-pathfinding
    // Adapted from http://theory.stanford.edu/~amitp/GameProgramming/
    private List<GraphPathData> GetPathAStar(GameObject from, GameObject to)
    {
        SortedDictionary<int, AStarInfo> openDict = new SortedDictionary<int, AStarInfo>();
        SortedDictionary<int, AStarInfo> closeDict = new SortedDictionary<int, AStarInfo>();

        AStarInfo currentNode = new AStarInfo(null, from, to, 0);
        closeDict.Add(currentNode.ID, currentNode);

        while( currentNode.node.GetInstanceID() != to.GetInstanceID() )
        {
            // Add neighbors to open dict
            MapGraphNode graphNode = nodes[closeDict[currentNode.ID].node.GetInstanceID()];
            foreach(GameObject neighbor in graphNode.Neighbors)
            {
                // If neighbor is in the closed list: Ignore it.
                if( !closeDict.ContainsKey(neighbor.GetInstanceID()) )
                {
                    AStarInfo neighborNode = new AStarInfo(graphNode.Vertex, neighbor, to, currentNode.Cost);
                    // If neighbor is not in the open list: Add it and compute its score.
                    if( !openDict.ContainsKey(neighbor.GetInstanceID()) )
                    {
                        openDict.Add(neighborNode.ID, neighborNode);
                    }
                    else
                    {
                        // If neighbor is already in the open list: 
                        //      Check if the COST is lower when we use the current generated path to get there.
                        //      If it is, update its score and update its parent as well.
                        AStarInfo comparingNode = openDict[neighbor.GetInstanceID()];
                        if( neighborNode.Cost < comparingNode.Cost )
                        {
                            openDict.Remove(comparingNode.ID);
                            openDict.Add(neighborNode.ID, neighborNode);
                        }
                    }
                }
            }

            // Get lowest score from open dict
            AStarInfo lowestScoreNode = openDict.OrderBy(key => key.Value.Cost).First().Value;
            // Remove S from the open list and add S to the closed list.
            openDict.Remove(lowestScoreNode.ID);
            closeDict.Add(lowestScoreNode.ID, lowestScoreNode);

            currentNode = lowestScoreNode;
        }

        // Going back to fetch the path
        List<GraphPathData> path = new List<GraphPathData>();

        AStarInfo destination = closeDict.OrderBy(pair => pair.Value.HeuristicDistanceToDestiny).First().Value;
        AStarInfo goingBackNode = closeDict[destination.previous.GetInstanceID()];

        path.Insert(0, new GraphPathData(goingBackNode.node));

        while(goingBackNode.previous != null)
        {
            path.Insert(0, new GraphPathData(goingBackNode.node));
            goingBackNode = closeDict[goingBackNode.previous.GetInstanceID()];
        }

        return path;
    }
}