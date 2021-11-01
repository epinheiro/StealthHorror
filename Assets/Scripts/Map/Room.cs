using System;
using UnityEngine;

public class Room : MonoBehaviour
{
    PolygonCollider2D roomCollider;
    RoomColliderPoints roomLimits;

    // public Transform position; // For debug, if necessary

    void Awake()
    {
        roomCollider = this.transform.Find("RoomCollider").GetComponent<PolygonCollider2D>();
        roomLimits = new RoomColliderPoints(roomCollider.points);
    }

    void Update()
    {
        // Debug.LogError($"Inside {IsPointInsideConvexPolygon(position.position)}");
    }

    // https://demonstrations.wolfram.com/AnEfficientTestForAPointToBeInAConvexPolygon/
    public bool IsPointInsideConvexPolygon(Vector2 point)
    {
        int numberOfPoints = roomLimits.crude.Length;

        for(int i=0; i<numberOfPoints; i++)
        {
            // Guarantees the original state
            roomLimits.trans[i] = roomLimits.crude[i];

            // Translate all polygon points considering point as origin
            roomLimits.trans[i] = roomLimits.crude[i] - point;

            // Translate all polygon points considering gameObject position
            roomLimits.trans[i] += ((Vector2)this.transform.position);
        }

        // Calculate angle between two points and check signs
        bool allPositive = true;
        bool allNegative = true;

        for(int i=0; i<numberOfPoints; i++)
        {
            Vector3 pointI = roomLimits.trans[i];
            Vector3 pointJ = roomLimits.trans[(i+1)%numberOfPoints];

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