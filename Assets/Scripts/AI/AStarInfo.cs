using UnityEngine;

public class AStarInfo
{
    public int ID => node.GetInstanceID();
    public GameObject previous;
    public GameObject node;
    public float Cost => DistanceFromOrigin + HeuristicDistanceToDestiny;
    public float DistanceFromOrigin {get; protected set;}
    public float HeuristicDistanceToDestiny {get; protected set;}

    public AStarInfo(GameObject previousNode, GameObject currentNode, GameObject finalNode, float weithUntilNow)
    {
        previous = previousNode;
        this.node = currentNode;
        HeuristicDistanceToDestiny = Vector3.Distance(currentNode.transform.position, finalNode.transform.position);
        DistanceFromOrigin = weithUntilNow;
    }

    public override string ToString()
    {
        return $"[{ID} {node.name}] [ ({previous?.name}) -> ({node?.name}) ] [{Cost}]";
    }
}