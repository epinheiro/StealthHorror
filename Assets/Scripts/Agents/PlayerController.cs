using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AgentController
{
    ///////////// Meta values /////////////
    protected override float SimpleModifier => 1f;
    protected override float RunningModifier => 1.75f;

    protected override Vector3 GetMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        return new Vector2(x, y);
    }

    protected override bool IsRunning()
    {
        return Input.GetButton("Fire3");
    }

    protected override bool IsInteracting()
    {
        return Input.GetButtonDown("Jump");
    }

    protected override void GetInitialRoom()
    {
        currentRoom = map.PlayerCurrentRoom;
    }
}
