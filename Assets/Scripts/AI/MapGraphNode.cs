using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGraphNode
{
    public GameObject Vertex {get; protected set;}
    public NodeType type {get; protected set;}
    public HashSet<GameObject> Neighbors;

    private void ConstructorBase(GameObject vertex)
    {
        this.Vertex = vertex;
        type = (NodeType)Enum.Parse(typeof(NodeType), vertex.tag, true);
        Neighbors = new HashSet<GameObject>();
    }

    public MapGraphNode(GameObject vertex)
    {
        ConstructorBase(vertex);
    }

    public MapGraphNode(GameObject vertex, GameObject adjacent)
    {
        ConstructorBase(vertex);
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