using System;
using UnityEngine;

public struct RoomColliderPoints
{
    public Vector2[] crude;
    public Vector2[] trans;

    public RoomColliderPoints(Vector2[] points)
    {
        crude = points;
        int arraySize = crude.Length;
        trans = new Vector2[arraySize];
        Array.Copy(crude, trans, crude.Length);
    }
}