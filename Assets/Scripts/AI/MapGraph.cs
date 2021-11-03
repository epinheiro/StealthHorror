using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGraph
{
    bool debugPrint = false;

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

            List<MapGraphEdge> edges = mapRef.GetEdges();
            if(debugPrint) Debug.LogError($"MapGraph Room {room.name} - edges [{edges.Count}]");
            foreach(MapGraphEdge edge in edges)
            {
                ProcessNode(edge.PointA, edge.PointB);
            }
        }
    }

    private void ProcessNode(GameObject point1, GameObject point2)
    {
        int point1ID = point1.gameObject.GetInstanceID();
        if ( !nodes.ContainsKey(point1ID) )
        {
            if(debugPrint) Debug.Log($"MapGraph ProcessNode {point1ID} - NEW [{point1.transform.parent.parent.name}] {point1.gameObject.name}");
            if(debugPrint) Debug.Log($"MapGraph ProcessNode {point1ID} - ADJACENT [{point1.transform.parent.parent.name}] {point1.gameObject.name} -> {point2.gameObject.GetInstanceID()} [{point2.transform.parent.parent.name}] {point2.gameObject.name}");
            nodes.Add(point1ID, new MapGraphNode(point1, point2));
            ProcessNode(point2, point1);
        }
        else
        {
            MapGraphNode node = nodes[point1ID];
            if ( !node.ContainsAdjacent(point2) )
            {
                if(debugPrint) Debug.Log($"MapGraph ProcessNode {point1ID} - ADJACENT [{point1.transform.parent.parent.name}] {point1.gameObject.name} -> {point2.gameObject.GetInstanceID()} [{point2.transform.parent.parent.name}] {point2.gameObject.name}");
                node.InsertAdjacent(point2);
                ProcessNode(point2, point1);
            }
        }
    }
}