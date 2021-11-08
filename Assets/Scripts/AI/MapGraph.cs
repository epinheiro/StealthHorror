using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGraph
{
    bool debugPrint = false;
    float debugDelay = 5f;
    bool debugPrintSearchGraph = false;

    List<Room> rooms;

    Dictionary<int, MapGraphNode> nodes;

    HashSet<MapGraph> graph;

    // Both rooms and mapReferences obeys same indexes, as one was made from the other
    public MapGraph(List<Room> rooms)
    {
        this.rooms = rooms;

        List<MapReference> references = new List<MapReference>();// Is it needed?
        nodes = new Dictionary<int, MapGraphNode>();
        foreach(Room room in rooms)
        {
            MapReference mapRef = room.MapReference;
            references.Add(mapRef); // Is it needed?

            /// Inner paths
            List<MapGraphEdge> edges = mapRef.GetEdges();
            if(debugPrint) Debug.LogError($"MapGraph Room {room.name} - edges [{edges.Count}] - adjacents rooms [{room.nodeToAdjacentDict.Count}]");
            foreach(MapGraphEdge edge in edges)
            {
                ProcessNode(edge.PointA, edge.PointB);
            }

            /// Inter paths
            foreach(KeyValuePair<Room, GameObject> pair in room.nodeToAdjacentDict)
            {
                GameObject point1 = room.GetTransitionNode(pair.Key);
                GameObject point2 = pair.Key.GetTransitionNode(room);
                ProcessNode(point1, point2);
                if(debugPrint)
                {
                    Debug.DrawLine(point1.transform.position, point2.transform.position, Color.magenta, debugDelay);
                    GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = pair.Value.transform.position;
                }
            }
        }
    }

    private void ProcessNode(GameObject point1, GameObject point2)
    {
        int point1ID = point1.gameObject.GetInstanceID();
        if ( !nodes.ContainsKey(point1ID) )
        {
            if(debugPrint) Debug.Log($"MapGraph ProcessNode {point1ID} - NEW [{point1.transform.parent.parent.name}] {point1.gameObject.name}");
            MapGraphNode newNode = new MapGraphNode(point1);
            nodes.Add(point1ID, newNode);

            InsertNeighbors(newNode, point1, point2);
        }
        else
        {
            MapGraphNode node = nodes[point1ID];
            if ( !node.ContainsAdjacent(point2) )
                InsertNeighbors(node, point1, point2);
        }
    }

    void InsertNeighbors(MapGraphNode node, GameObject point1, GameObject point2)
    {
        int point1ID = point1.gameObject.GetInstanceID();
        if(debugPrint) Debug.Log($"MapGraph ProcessNode {point1ID} - NEIGHBOR [{point1.transform.parent.parent.name}] {point1.gameObject.name} -> {point2.gameObject.GetInstanceID()} [{point2.transform.parent.parent.name}] {point2.gameObject.name}");
        node.InsertNeighbors(point2);
        ProcessNode(point2, point1);
    }

    public MapGraphNode GetNodeInfo(GameObject goNode)
    {
        MapGraphNode graphNode;
        nodes.TryGetValue(goNode.GetInstanceID(), out graphNode);
        return graphNode;
    }

    public List<GameObject> GetPath(GameObject agent, Vector3 toPosition)
    {
        GameObject fromNode = GetClosestNode(agent.transform.position);
        GameObject toNode = GetClosestNode(toPosition);

        List<GameObject> path = GetPath(fromNode, toNode);
        path.Insert(0, fromNode); // Guarantee the first node

        return path;
    }

    public List<GameObject> GetRandomPath(GameObject agent)
    {
        return GetPath(agent, GetRandomNode().Vertex.transform.position);
    }

    MapGraphNode GetRandomNode()
    {
        return nodes.ElementAt(UnityEngine.Random.Range(0, nodes.Keys.Count)).Value;
    }

    private List<GameObject> GetPath(GameObject from, GameObject to)
    {
        List<GameObject> path;
        GetPathRandomDepthRecursive(from, to, out path);

        if(debugPrintSearchGraph)
        {
            Debug.DrawLine(from.transform.position, to.transform.position, Color.blue, debugDelay+1);
            if (path != null)
            {
                string pathMsg = $"SEARCH RESULT {from.name} | ";
                if(path.Count > 0) Debug.DrawLine(from.transform.position, path[0].transform.position, Color.cyan, debugDelay+1);
                for (int i = 0; i < path.Count-1; i++)
                {
                    pathMsg += $"{path[i].name} | ";
                    Debug.DrawLine(path[i].transform.position, path[i + 1].transform.position, Color.cyan, debugDelay+1);
                }
                pathMsg += $"{path[path.Count-1].name}";
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

    private bool GetPathRandomDepthRecursive(GameObject from, GameObject to, out List<GameObject> path, int depth = 0, HashSet<int> visited = null)
    {
        int fromInstanceID = from.GetInstanceID();
        int toInstanceID = to.GetInstanceID();

        if(fromInstanceID == toInstanceID)
        {
            if(debugPrintSearchGraph) Debug.LogError($"SEARCH - FOUND in depth {depth}");
            path = new List<GameObject>();
            path.Add(to);
            return true;
        }

        if(visited == null)
            visited = new HashSet<int>();
  
        if( !visited.Contains(fromInstanceID) )
            visited.Add(fromInstanceID);
        else
        {
            if(debugPrintSearchGraph) Debug.LogAssertion($"SEARCH - already visited {from.name}");
            path = null;
            return false;
        }

        MapGraphNode graphNode = nodes[fromInstanceID];
        foreach(GameObject neighbor in graphNode.Neighbors)
        {
            if(debugPrintSearchGraph)
            {
                Debug.Log($"SEARCH - go {from.name} [{from.gameObject.GetInstanceID()}] to {neighbor.name} [{neighbor.gameObject.GetInstanceID()}]");
                Debug.DrawLine(from.transform.position, neighbor.transform.position, Color.black, debugDelay);
            }
            MapGraphNode neighborGraphNode = nodes[neighbor.GetInstanceID()];

            if( from.gameObject.GetInstanceID() != neighbor.gameObject.GetInstanceID() )
            {
                if(GetPathRandomDepthRecursive(neighbor, to, out path, depth++, visited))
                {
                    path.Insert(0, neighbor);
                    return true;
                }
            }
        }

        path = null;
        return false;
    }

    // Adapted from https://www.raywenderlich.com/3016-introduction-to-a-pathfinding
    // Adapted from http://theory.stanford.edu/~amitp/GameProgramming/
    private List<GameObject> GetPathAStar(GameObject from, GameObject to)
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
        List<GameObject> path = new List<GameObject>();

        AStarInfo destination = closeDict.OrderBy(pair => pair.Value.HeuristicDistanceToDestiny).First().Value;
        AStarInfo goingBackNode = closeDict[destination.previous.GetInstanceID()];

        path.Insert(0, goingBackNode.node);

        while(goingBackNode.previous != null)
        {
            path.Insert(0, goingBackNode.node);
            goingBackNode = closeDict[goingBackNode.previous.GetInstanceID()];
        }

        return path;
    }
}