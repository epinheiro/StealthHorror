using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : AgentController
{
    MonsterAI ai;

    GameObject goingTo;

    protected override void GetInitialRoom()
    {
        currentRoom = map.MonsterCurrentRoom;
    }

    public GameObject GoToTest; // For debug, if necessary

    protected override void Update()
    {
        if(ai == null)
            ai = new MonsterAI(map.Graph);

        if(!ai.HavePath)
        {
            ai.CreatePath(this.gameObject, GoToTest);
            ai.GetNextNode(true); // FIXME - first node is always the destination!
        }

        if( ai.IsCurrentNodeClose(this.transform.position) )
        {
            ai.GetNextNode(true);
        }
        else
        {
            Vector3 movVector = (ai.CurrentNodePosition - this.transform.position).normalized;
            Vector3 nextPositon = this.transform.position + movVector * SimpleModifier * (IsRunning() ? RunningModifier : SimpleModifier) *  Time.deltaTime;

            // if(currentRoom.IsPointInsideConvexPolygon(nextPositon))
            // {
            this.transform.position = nextPositon;
                // adjacentRoom = currentRoom.IsPointInRoomTransition(nextPositon);
                // canMakeTransition = adjacentRoom != null;
            //     if(canMakeTransition) Debug.LogError($"IsPointInRoomTransition [{currentRoom.IsPointInRoomTransition(nextPositon).name}]");
            // }
        }
    }
}
