using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGraphNode
{
    public GameObject Vertex {get; protected set;}
    public NodeType type {get; protected set;}
    public HashSet<GameObject> Neighbors;

    public MapGraphNode(GameObject vertex, GameObject adjacent)
    {
        this.Vertex = vertex;
        type = (NodeType) Enum.Parse(typeof(NodeType), vertex.tag, true);
        Neighbors = new HashSet<GameObject>();
        Neighbors.Add(adjacent);
    }

    public bool ContainsAdjacent(GameObject adjacent)
    {
        return Neighbors.Contains(adjacent);
    }

    public bool InsertNeighbors(GameObject adjacent)
    {
        return Neighbors.Add(adjacent);
    }
}