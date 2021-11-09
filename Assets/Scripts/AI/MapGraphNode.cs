using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGraphNode
{
    public Room Room {get; protected set;}
    public GameObject Vertex {get; protected set;}
    public NodeType type {get; protected set;}
    public List<GameObject> Neighbors;
    public List<GameObject> ShuffledNeighbors => Shuffle(Neighbors);

    private void ConstructorBase(GameObject vertex, Room room)
    {
        this.Room = room;
        this.Vertex = vertex;
        type = (NodeType)Enum.Parse(typeof(NodeType), vertex.tag, true);
        Neighbors = new List<GameObject>();
    }

    public MapGraphNode(GameObject vertex, Room room)
    {
        ConstructorBase(vertex, room);
    }

    public bool ContainsAdjacent(GameObject adjacent)
    {
        return Neighbors.Contains(adjacent);
    }

    public void InsertNeighbors(GameObject adjacent)
    {
        Neighbors.Add(adjacent);
    }

    private System.Random rng = new System.Random(DateTime.Now.Second);

    private List<T> Shuffle<T>(List<T> list)
    {
        T[] shuffled = new T[list.Count];
        list.CopyTo(shuffled);

        int n = shuffled.Length;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            T value = shuffled[k];
            shuffled[k] = shuffled[n];
            shuffled[n] = value;
        }

        return new List<T>(shuffled);
    }
}