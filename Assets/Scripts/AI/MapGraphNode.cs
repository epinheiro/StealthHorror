using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGraphNode
{
    public GameObject Vertex {get; protected set;}
    public NodeType type {get; protected set;}
    HashSet<GameObject> adjacentVertexes;

    public MapGraphNode(GameObject vertex, GameObject adjacent)
    {
        this.Vertex = vertex;
        type = (NodeType) Enum.Parse(typeof(NodeType), vertex.tag, true);
        adjacentVertexes = new HashSet<GameObject>();
        adjacentVertexes.Add(adjacent);
    }

    public bool ContainsAdjacent(GameObject adjacent)
    {
        return adjacentVertexes.Contains(adjacent);
    }

    public bool InsertAdjacent(GameObject adjacent)
    {
        return adjacentVertexes.Add(adjacent);
    }
}