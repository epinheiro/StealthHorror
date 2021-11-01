using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    PolygonCollider2D roomCollider;

    List<RoomColliderPoints> roomLimitsList;

    // public Transform position; // For debug, if necessary

    void Awake()
    {
        roomLimitsList = new List<RoomColliderPoints>();

        PolygonCollider2D[] colliders = this.transform.Find("RoomCollider").GetComponents<PolygonCollider2D>();
        foreach(PolygonCollider2D collider in colliders)
        {
            roomLimitsList.Add(new RoomColliderPoints(collider.points));
        }
    }

    void Update()
    {
        // Debug.LogError($"Inside {IsPointInsideConvexPolygon(position.position)}");
    }

    // https://demonstrations.wolfram.com/AnEfficientTestForAPointToBeInAConvexPolygon/
    public bool IsPointInsideConvexPolygon(Vector2 point)
    {
        bool result = false;

        foreach(RoomColliderPoints roomPoints in roomLimitsList)
        {
            int numberOfPoints = roomPoints.crude.Length;

            for(int i=0; i<numberOfPoints; i++)
            {
                // Guarantees the original state
                roomPoints.trans[i] = roomPoints.crude[i];

                // Translate all polygon points considering point as origin
                roomPoints.trans[i] = roomPoints.crude[i] - point;

                // Translate all polygon points considering gameObject position
                roomPoints.trans[i] += ((Vector2)this.transform.position);
            }

            // Calculate angle between two points and check signs
            bool allPositive = true;
            bool allNegative = true;

            for(int i=0; i<numberOfPoints; i++)
            {
                Vector3 pointI = roomPoints.trans[i];
                Vector3 pointJ = roomPoints.trans[(i+1)%numberOfPoints];

                float operation = pointJ.x*pointI.y - pointI.x*pointJ.y;
                bool angleSignPositive = operation > 0;

                // { // DEBUG ///////////////////
                //     Debug.Log($"Angle {pointI} {pointJ} - op {operation} - positive {angleSignPositive}");
                //     Debug.DrawLine(pointI, pointI, Color.magenta, 0.1f);
                //     GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = pointI;
                // } // DEBUG ///////////////////

                allPositive &= angleSignPositive;
                allNegative &= !angleSignPositive;
            }

            result |= allPositive || allNegative;
        }
        return result;
    }
}