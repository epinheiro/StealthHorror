using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public GameObject PointA {get; protected set;}
    public GameObject PointB {get; protected set;}

    public Edge(GameObject pointA, GameObject pointB)
    {
        this.PointA = pointA;
        this.PointB = pointB;
    }
}
