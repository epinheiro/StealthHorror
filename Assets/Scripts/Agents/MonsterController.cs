using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : AgentController
{
    protected override void GetInitialRoom()
    {
        currentRoom = map.MonsterCurrentRoom;
    }
}
