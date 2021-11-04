using System.Collections;
using System.Collections.Generic;
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

    public List<GameObject> GetPath(GameObject from, GameObject to)
    {
        List<GameObject> path;
        GetPathDepthRecursive(from, to, out path);

        if(debugPrintSearchGraph)
        {
            Debug.DrawLine(from.transform.position, to.transform.position, Color.blue, debugDelay+1);
            if (path != null)
            {
                string pathMsg = $"SEARCH RESULT {from.name} | ";
                if(path.Count > 0) Debug.DrawLine(from.transform.position, path[path.Count-1].transform.position, Color.cyan, debugDelay+1);
                for (int i = path.Count - 1; i > 0; i--)
                {
                    pathMsg += $"{path[i].name} | ";
                    Debug.DrawLine(path[i].transform.position, path[i - 1].transform.position, Color.cyan, debugDelay+1);
                }
                pathMsg += $"{path[0].name}";
                Debug.LogAssertion(pathMsg);
            }
        }

        return path;
    }

    private bool GetPathDepthRecursive(GameObject from, GameObject to, out List<GameObject> path, int depth = 0, HashSet<int> visited = null)
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
                if(GetPathDepthRecursive(neighbor, to, out path, depth++, visited))
                {
                    path.Add(neighbor);
                    return true;
                }
            }
        }

        path = null;
        return false;
    }
}