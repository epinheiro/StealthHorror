using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    ///////////// Meta values /////////////
    protected virtual float SimpleModifier => 1.75f;
    protected virtual float RunningModifier => 2.25f;

    ///////////// Control references /////////////
    [SerializeField] protected LayoutController map;
    protected Room currentRoom;
    protected Room adjacentRoom;
    protected bool canMakeTransition;

    void Awake()
    {
        GetInitialRoom();
    }

    void Update()
    {
        Vector3 movVector = GetMovement();

        Vector3 nextPositon = this.transform.position + movVector * SimpleModifier * (IsRunning() ? RunningModifier : 1) * Time.deltaTime;

        if(currentRoom.IsPointInsideConvexPolygon(nextPositon))
        {
            this.transform.position = nextPositon;
            adjacentRoom = currentRoom.IsPointInRoomTransition(nextPositon);
            canMakeTransition = adjacentRoom != null;
            // if(canMakeTransition) Debug.LogError($"IsPointInRoomTransition [{currentRoom.IsPointInRoomTransition(nextPositon).name}]");
        }

        if (canMakeTransition && IsInteracting())
        {
            map.GoToRoom(adjacentRoom);
            currentRoom = adjacentRoom;
        }
    }

    protected virtual Vector3 GetMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        return new Vector2(x, y);
    }

    protected virtual bool IsRunning()
    {
        return Input.GetButton("Fire3");
    }

    protected virtual bool IsInteracting()
    {
        return Input.GetButtonDown("Jump");
    }

    protected virtual void GetInitialRoom()
    {
        currentRoom = map.MonsterCurrentRoom;
    }
}
