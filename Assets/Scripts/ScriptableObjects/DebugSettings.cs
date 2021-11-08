using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DebugSettings", menuName = "ScriptableObjects/Debug", order = 1)]
public class DebugSettings : ScriptableObject
{
    public bool AlwaysShowSprites = false;
    public bool ShowPathFindingGraphs = false;
    public bool ShowContructingRoomGraph = false;
}
