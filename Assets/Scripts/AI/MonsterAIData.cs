using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MonsterAIData
{
    public Vector3 movement;
    public bool isRunning;
    public Room room;

    public MonsterAIData(Room room, Vector3? movement = null, bool isRunning = false)
    {
        this.room = room;
        this.movement = movement.HasValue ? movement.Value : Vector3.zero;
        this.isRunning = isRunning;
    }
}
