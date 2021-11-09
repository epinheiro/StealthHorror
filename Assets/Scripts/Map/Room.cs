using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Room : MonoBehaviour
{
    //// Configuration
    readonly float hidingSpotProximityCheck = 0.5f;

    PolygonCollider2D roomCollider;

    List<RoomColliderPoints> roomLimitsList;

    [SerializeField] Room[] adjacentRooms;

    [SerializeField] GameObject[] nodeToAdjacent;

    [SerializeField] GameObject[] hidingPlaces;

    public Dictionary<Room, GameObject> nodeToAdjacentDict;

    public MapReference MapReference {get; protected set;}

    // public Transform position; // For debug, if necessary

    void Awake()
    {
        roomLimitsList = new List<RoomColliderPoints>();
        nodeToAdjacentDict = new Dictionary<Room, GameObject>();

        PolygonCollider2D[] colliders = this.transform.Find("RoomCollider").GetComponents<PolygonCollider2D>();
        foreach(PolygonCollider2D collider in colliders)
        {
            roomLimitsList.Add(new RoomColliderPoints(collider.points));
        }

        MapReference = this.transform.Find("MapReference").GetComponent<MapReference>();

        if(adjacentRooms.Length == nodeToAdjacent.Length)
        {
            for(int i=0; i<adjacentRooms.Length; i++)
            {
                nodeToAdjacentDict.Add(adjacentRooms[i], nodeToAdjacent[i]);
            }
        }
        else
        {
            Debug.LogError($"adjacentRooms[{adjacentRooms.Length}] and nodeToAdjacent [{nodeToAdjacent.Length}] do not have the same lenght to make the dictionary");
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

    public Room IsPointInRoomTransition(Vector2 point)
    {
        foreach(Room room in adjacentRooms)
        {
            if(room.IsPointInsideConvexPolygon(point))
                return room;
        }
        return null;
    }

    public bool IsPointNearHidingPlace(Vector3 position)
    {
        foreach(GameObject hidingPlace in hidingPlaces)
        {
            if( Vector2.Distance(hidingPlace.transform.position, position) <= hidingSpotProximityCheck )
            {
                return true;
            }
        }
        return false;
    }

    public Room GetTransitionToRoom(GameObject node)
    {
        return this.nodeToAdjacentDict.First(x => x.Value.gameObject.GetInstanceID() == node.GetInstanceID()).Key;
    }

    public GameObject GetTransitionNode(Room nodeToTransition)
    {
        GameObject gameObject;
        this.nodeToAdjacentDict.TryGetValue(nodeToTransition, out gameObject);
        return gameObject;
    }
}