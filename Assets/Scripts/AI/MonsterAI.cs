using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI
{
    PlayerController playerController;
    MonsterController controller;
    MapGraph graph;

    GraphPathData currentPathStep;
    public Vector3 CurrentPathStepPosition => currentPathStep.position;

    public bool HavePath => currentPath != null && currentPath.Count > 0;
    List<GraphPathData> currentPath;

    float closeVerification = 0.1f;

    public MonsterAI(MonsterController controller, MapGraph graph)
    {
        this.controller = controller;
        this.graph = graph;
        playerController = GameManager.Instance.Player;
    }

    public MonsterAIData Update(MonsterController controller)
    {
        MonsterAIData data = new MonsterAIData();

        bool sameRoom = controller.CurrentRoom == playerController.CurrentRoom;

        if(sameRoom)
        {
            switch(playerController.State)
            {
                case PlayerState.Hiding:
                    Debug.DrawLine(controller.transform.position, playerController.transform.position, Color.white, 0.1f);
                    break;
                case PlayerState.Visible:
                    Debug.DrawLine(controller.transform.position, playerController.transform.position, Color.grey, 0.1f);
                    break;
            }
            
        }

        if( !HavePath )
            CreatePathToRandomLocation(controller.gameObject);

        if( IsCurrentNodeClose(controller.transform.position) )
        {
            GraphPathData nextNodeObject = GetNextPosition(true);
            MapGraphNode nextNode = graph.GetNodeInfo(nextNodeObject);
            if( nextNode != null)
            {
                data.movement = Vector3.zero;
                data.room = nextNode.Room;
            }
            else
            {
                data.movement = nextNodeObject.position;
            }
        }
        else
        {
            data.movement = (CurrentPathStepPosition - controller.transform.position).normalized;
        }

        return data;
    }

    public void ProcessSound(SoundType soundType, Vector3 position, Room room)
    {
        switch(soundType)
        {
            case SoundType.OpenDoor:
                CreatePath(controller.gameObject, position);
                break;
        }
    }

    private void CreatePathToRandomLocation(GameObject agent)
    {
        SetPath(graph.GetRandomPath(agent));
    }

    private void CreatePath(GameObject agent, Vector3 to)
    {
        SetPath(graph.GetMinimumPath(agent, to));
    }

    private void CreatePath(GameObject agent, GameObject to)
    {
        SetPath(graph.GetMinimumPath(agent, to.transform.position));
    }

    private void SetPath(List<GraphPathData> path)
    {
        currentPath = path;
        currentPathStep = currentPath[0];
    }

    private GraphPathData GetNextPosition(bool remove = false)
    {
        if(currentPath.Count > 0)
        {
            GraphPathData go = currentPath[0];
            if(remove) currentPath.RemoveAt(0);
            currentPathStep = go;
            return go;
        }
        return null;
    }

    private bool IsCurrentNodeClose(Vector3 position)
    {
        float distance = Vector2.Distance(position, currentPathStep.position);
        return distance <= closeVerification;
    }

    private bool IsCurrentNodeTheLast()
    {
        return currentPathStep != null && currentPath.Count == 1;
    }

    private bool IsNodeDoor(GameObject go)
    {
        return go.CompareTag("Door");
    }
}
