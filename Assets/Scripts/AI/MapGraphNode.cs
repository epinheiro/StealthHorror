using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGraphNode
{
    public GameObject Vertex {get; protected set;}
    public NodeType type {get; protected set;}
    HashSet<GameObject> neighbors;

    public MapGraphNode(GameObject vertex, GameObject adjacent)
    {
        this.Vertex = vertex;
        type = (NodeType) Enum.Parse(typeof(NodeType), vertex.tag, true);
        neighbors = new HashSet<GameObject>();
        neighbors.Add(adjacent);
    }

    public bool ContainsAdjacent(GameObject adjacent)
    {
        return neighbors.Contains(adjacent);
    }

    public bool InsertNeighbors(GameObject adjacent)
    {
        return neighbors.Add(adjacent);
    }
}