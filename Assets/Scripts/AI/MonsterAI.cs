using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI
{
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
    }

    public Vector3 Update(MonsterController controller)
    {
        if( !HavePath )
            CreatePathToRandomLocation(controller.gameObject);

        if( IsCurrentNodeClose(controller.transform.position) )
        {
            GetNextNode(true);
        }
        else
        {
            return (CurrentNodePosition - controller.transform.position).normalized;
        }

        return Vector3.zero;
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
}
