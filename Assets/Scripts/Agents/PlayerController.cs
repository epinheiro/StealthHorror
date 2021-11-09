using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    ///////////// Meta values /////////////
    protected float SimpleModifier => 1f;
    protected float RunningModifier => 1.75f;

    ///////////// Control references /////////////
    [SerializeField] protected LayoutController map;
    public Room CurrentRoom {get; protected set;}
    public Action<Room> ChangingRoom;
    protected Room adjacentRoom;
    protected bool canMakeTransition;

    void Awake()
    {
        GetInitialRoom();
    }

    protected void GetInitialRoom()
    {
        CurrentRoom = map.PlayerCurrentRoom;
    }

    protected virtual void Update()
    {
        Vector3 movVector = GetMovement();

        Vector3 nextPositon = this.transform.position + movVector * (IsRunning() ? RunningModifier : SimpleModifier) * Time.deltaTime;

        if(CurrentRoom.IsPointInsideConvexPolygon(nextPositon))
        {
            this.transform.position = nextPositon;
            adjacentRoom = CurrentRoom.IsPointInRoomTransition(nextPositon);
            canMakeTransition = adjacentRoom != null;
            // if(canMakeTransition) Debug.LogError($"IsPointInRoomTransition [{currentRoom.IsPointInRoomTransition(nextPositon).name}]");
        }

        if (canMakeTransition && IsInteracting())
        {
            SoundCommunicationLayer.instance.MakeSound(SoundType.OpenDoor, this.transform.position, CurrentRoom);
            ChangingRoom?.Invoke(adjacentRoom);
            CurrentRoom = adjacentRoom;
        }
    }

    protected Vector3 GetMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        return new Vector2(x, y);
    }

    protected bool IsRunning()
    {
        return Input.GetButton("Fire3");
    }

    protected bool IsInteracting()
    {
        return Input.GetButtonDown("Jump");
    }

}
