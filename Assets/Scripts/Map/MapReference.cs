using UnityEditor;
using UnityEngine;

public class MapReference : MonoBehaviour
{
    public GameObject[] pointsA;
    public GameObject[] pointsB;

    public Edge GetEdge(int index)
    {
        return new Edge(pointsA[index], pointsB[index]);
    }
}

// Based on https://docs.unity3d.com/ScriptReference/Handles.DrawLine.html
[CustomEditor(typeof(MapReference))]
class MapReferenceSceneEditor : Editor
{
    public static void DrawEdges(MapReference map)
    {
        Handles.color = Color.red;

        if(map.pointsA == null || map.pointsB == null || map.pointsA.Length != map.pointsB.Length)
        {
            Debug.LogError($"{map.transform.parent.name}[{map.name}] does not have points A and B at same lenght!");
            return;
        }

        bool hadError = false;
        string errorMessage = $"{map.transform.parent.name}[{map.name}] does not have correct vertixes in indexes: ";

        for (int i = 0; i < map.pointsA.Length; i++)
        {
            if(map.pointsA[i] != null && map.pointsB[i])
                Handles.DrawLine(map.pointsA[i].transform.position, map.pointsB[i].transform.position, 3);
            else
            {
                hadError = true;
                errorMessage += $"{i}, ";
            }
        }

        if(hadError) Debug.LogError(errorMessage);
    }

    void OnSceneGUI()
    {
        DrawEdges(target as MapReference);
    }
}