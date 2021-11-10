using UnityEngine;

public class GraphPathData
{
    readonly bool OnlyPosition;

    readonly string _name;
    public string Name => OnlyPosition ? "" : _name;

    readonly string _tag;
    public string Tag => OnlyPosition ? "" : _tag;

    readonly int _nodeID;
    public int NodeInstanceID => OnlyPosition ? 0 : _nodeID;

    public readonly Vector3 position;

    public GraphPathData(Vector3 position)
    {
        this.position = position;
        OnlyPosition = true;
    }

    public GraphPathData(GameObject go)
    {
        this._name = go.name;
        this._tag = go.tag;
        this._nodeID = go.GetInstanceID();
        this.position = go.transform.position;
        OnlyPosition = false;
    }
}
