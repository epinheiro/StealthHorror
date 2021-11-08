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

    ///////////// Systemic /////////////
    SoundListener listener;

    MonsterAI _ai;
    MonsterAI AI
    {
        get 
        { 
            if(_ai == null)
                _ai = new MonsterAI(this, map.Graph);
            return _ai;
        }
    }

    GameObject goingTo;
    GameObject visualObject;

    void Awake()
    {
        GetInitialRoom();
        visualObject = this.transform.Find("Visual").gameObject;
        GetComponent<SoundListener>().SoundEvent += ProcessSound;
    }

    protected void GetInitialRoom()
    {
        currentRoom = map.MonsterCurrentRoom;
    }

    void Update()
    {
        MonsterAIData data = AI.Update(this);
        Vector3 nextPositon = this.transform.position + data.movement * SimpleModifier * (data.isRunning ? RunningModifier : SimpleModifier) *  Time.deltaTime;

        this.transform.position = nextPositon;

        if( GameManager.Instance.DebugSettings.AlwaysShowSprites )
        {
            ChangeVisibility(true);
        }
        else
        {
            bool isVisibile = map.PlayerCurrentRoom.IsPointInsideConvexPolygon(this.transform.position);
            ChangeVisibility(isVisibile);
        }
    }

    private void ChangeVisibility(bool isVisibile)
    {
        visualObject.SetActive(isVisibile);
    }

    protected void ProcessSound(SoundType soundType, Vector3 position, Room room)
    {
        AI.ProcessSound(soundType, position, room);
    }
}
