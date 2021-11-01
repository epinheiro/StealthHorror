using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    PolygonCollider2D roomCollider;
    Vector2[] originalRoomLimits;
    Vector2[] translatedRoomLimits;

    // public Transform position; // For debug, if necessary

    void Awake()
    {
        roomCollider = this.transform.Find("RoomCollider").GetComponent<PolygonCollider2D>();
        originalRoomLimits = roomCollider.points;
        int arraySize = originalRoomLimits.Length;
        translatedRoomLimits = new Vector2[arraySize];
        Array.Copy(originalRoomLimits, translatedRoomLimits, originalRoomLimits.Length);
    }

    void Update()
    {
        // Debug.LogError($"Inside {IsPointInsideConvexPolygon(position.position)}");
    }

    // https://demonstrations.wolfram.com/AnEfficientTestForAPointToBeInAConvexPolygon/
    public bool IsPointInsideConvexPolygon(Vector2 point)
    {
        int numberOfPoints = originalRoomLimits.Length;

        for(int i=0; i<numberOfPoints; i++)
        {
            // Guarantees the original state
            translatedRoomLimits[i] = originalRoomLimits[i];

            // Translate all polygon points considering point as origin
            translatedRoomLimits[i] = originalRoomLimits[i] - point;

            // Translate all polygon points considering gameObject position
            translatedRoomLimits[i] += ((Vector2)this.transform.position);
        }

        // Calculate angle between two points and check signs
        bool allPositive = true;
        bool allNegative = true;

        for(int i=0; i<numberOfPoints; i++)
        {
            Vector3 pointI = translatedRoomLimits[i];
            Vector3 pointJ = translatedRoomLimits[(i+1)%numberOfPoints];

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

        return allPositive || allNegative;
    }
}
