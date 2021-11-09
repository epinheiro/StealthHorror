using System;
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
    public Room CurrentRoom {get; protected set;}
    public Action<Room> ChangingRoom;
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

    PlayerController playerController;


    void Awake()
    {
        GetInitialRoom();
        visualObject = this.transform.Find("Visual").gameObject;
        GetComponent<SoundListener>().SoundEvent += ProcessSound;
        playerController = GameManager.Instance.Player;
    }

    protected void GetInitialRoom()
    {
        CurrentRoom = map.MonsterCurrentRoom;
    }

    void Update()
    {
        MonsterAIData data = AI.Update(this);
        Vector3 nextPositon = this.transform.position + data.movement * SimpleModifier * (data.isRunning ? RunningModifier : SimpleModifier) *  Time.deltaTime;

        this.transform.position = nextPositon;

        Room goingToRoom = data.room;
        if(goingToRoom != null && goingToRoom != CurrentRoom)
        {
            CurrentRoom = goingToRoom;
            ChangingRoom?.Invoke(CurrentRoom);
        }

        bool sameRoom = this.CurrentRoom == playerController.CurrentRoom;

        if (GameManager.Instance.DebugSettings.AlwaysShowSprites)
            ChangeVisibility(true);
        else
            ChangeVisibility(sameRoom);
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
