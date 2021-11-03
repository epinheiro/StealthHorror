using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGraph
{
    bool debugPrint = false;
    float debugDelay = 5f;

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
                if(debugPrint) Debug.DrawLine(edge.PointA.transform.position, edge.PointB.transform.position, Color.black, debugDelay);
            }

            /// Inter paths
            foreach(KeyValuePair<Room, GameObject> pair in room.nodeToAdjacentDict)
            {
                ProcessNode(room.GetTransitionNode(pair.Key), pair.Key.GetTransitionNode(room));
                if(debugPrint) GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = pair.Value.transform.position;
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
}