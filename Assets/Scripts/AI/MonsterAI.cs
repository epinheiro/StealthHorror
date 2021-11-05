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

    public void CreatePath(GameObject currentPosition, GameObject to)
    {
        currentPath = graph.GetPath(currentPosition.transform.position, to.transform.position);
        currentNode = currentPath[0];
    }

    public GameObject GetNextNode(bool remove = false)
    {
        if(currentPath.Count > 0)
        {
            GameObject go = currentPath[currentPath.Count-1];
            if(remove) currentPath.RemoveAt(currentPath.Count-1);
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
