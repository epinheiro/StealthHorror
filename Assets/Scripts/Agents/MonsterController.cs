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

    MonsterAI _ai;
    MonsterAI AI
    {
        get 
        { 
            if(_ai == null)
                _ai = new MonsterAI(map.Graph);
            return _ai;
        }
    }

    GameObject goingTo;
    GameObject visualObject;

    void Awake()
    {
        GetInitialRoom();
        visualObject = this.transform.Find("Visual").gameObject;
    }

    protected void GetInitialRoom()
    {
        currentRoom = map.MonsterCurrentRoom;
    }

    void Update()
    {
        if(!AI.HavePath)
            AI.CreatePathToRandomLocation(this.gameObject);

        if( AI.IsCurrentNodeClose(this.transform.position) )
        {
            AI.GetNextNode(true);
        }
        else
        {
            Vector3 movVector = (AI.CurrentNodePosition - this.transform.position).normalized;
            Vector3 nextPositon = this.transform.position + movVector * SimpleModifier * (IsRunning() ? RunningModifier : SimpleModifier) *  Time.deltaTime;

            this.transform.position = nextPositon;
        }

        bool isVisibile = map.PlayerCurrentRoom.IsPointInsideConvexPolygon(this.transform.position);
        visualObject.SetActive( isVisibile );
    }

    protected bool IsRunning()
    {
        return true; // TODO - AI behavior
    }
}
