using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI
{
    MapGraph graph;

    GameObject currentNode;
    public Vector3 CurrentNodePosition => currentNode.transform.position;

    public bool HavePath => currentPath != null && currentPath.Count > 0;
    List<GameObject> currentPath;

    float closeVerification = 0.3f;

    public MonsterAI(MapGraph graph)
    {
        this.graph = graph;
    }

    public void CreatePathToRandomLocation(GameObject agent)
    {
        SetPath(graph.GetRandomPath(agent));
    }

    public void CreatePath(GameObject agent, GameObject to)
    {
        SetPath(graph.GetPath(agent, to.transform.position));
    }

    void SetPath(List<GameObject> path)
    {
        currentPath = path;
        currentNode = currentPath[0];
    }

    public GameObject GetNextNode(bool remove = false)
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

    public bool IsCurrentNodeClose(Vector3 position)
    {
        float distance = Vector2.Distance(position, currentNode.transform.position);
        return distance <= closeVerification;
    }
}
