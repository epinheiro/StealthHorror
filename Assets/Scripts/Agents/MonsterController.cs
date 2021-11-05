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
    GameObject visualObject;

    public GameObject GoToTest; // For debug, if necessary

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
        if(ai == null)
            ai = new MonsterAI(map.Graph);

        if(!ai.HavePath)
            ai.CreatePath(this.gameObject, GoToTest);

        if( ai.IsCurrentNodeClose(this.transform.position) )
        {
            ai.GetNextNode(true);
        }
        else
        {
            Vector3 movVector = (ai.CurrentNodePosition - this.transform.position).normalized;
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
