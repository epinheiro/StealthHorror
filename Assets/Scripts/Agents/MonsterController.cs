using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    ///////////// Meta values /////////////
    protected virtual float SimpleModifier => 1.75f;
    protected virtual float RunningModifier => 2.25f;

    ///////////// Control references /////////////
    [SerializeField] protected LayoutController map;
    protected Room currentRoom;
    protected Room adjacentRoom;
    protected bool canMakeTransition;

    MonsterAI ai;

    GameObject goingTo;

    public GameObject GoToTest; // For debug, if necessary

    void Awake()
    {
        GetInitialRoom();
    }

    protected void GetInitialRoom()
    {
        currentRoom = map.MonsterCurrentRoom;
    }

    void Update()
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

    protected bool IsRunning()
    {
        return true; // TODO - AI behavior
    }
}
