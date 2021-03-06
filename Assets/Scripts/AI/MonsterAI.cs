using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI
{
    PlayerController playerController;
    MonsterController controller;
    MapGraph graph;

    GameObject currentNode;
    public Vector3 CurrentNodePosition => currentNode.transform.position;

    public bool HavePath => currentPath != null && currentPath.Count > 0;
    List<GameObject> currentPath;

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
            GameObject nextNodeObject = GetNextNode(true);
            MapGraphNode nextNode = graph.GetNodeInfo(nextNodeObject);
            data.movement = Vector3.zero;
            data.room = nextNode.Room;
        }
        else
        {
            data.movement = (CurrentNodePosition - controller.transform.position).normalized;
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

    private void SetPath(List<GameObject> path)
    {
        currentPath = path;
        currentNode = currentPath[0];
    }

    private GameObject GetNextNode(bool remove = false)
    {
        if(currentPath.Count > 0)
        {
            GameObject go = currentPath[0];
            if(remove) currentPath.RemoveAt(0);
            currentNode = go;
            return go;
        }
        return null;
    }

    private bool IsCurrentNodeClose(Vector3 position)
    {
        float distance = Vector2.Distance(position, currentNode.transform.position);
        return distance <= closeVerification;
    }

    private bool IsCurrentNodeTheLast()
    {
        return currentNode != null && currentPath.Count == 1;
    }

    private bool IsNodeDoor(GameObject go)
    {
        return go.CompareTag("Door");
    }
}
