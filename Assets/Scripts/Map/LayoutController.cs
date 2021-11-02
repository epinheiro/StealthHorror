using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LayoutController : MonoBehaviour
{
    public Room CurrentRoom;
    [SerializeField] public List<Room> Map;

    void Awake()
    {
        MakeOnlyVisibleCurrentRoom();
    }

    private void MakeOnlyVisibleCurrentRoom()
    {
        foreach (Room room in Map)
        {
            if (room != CurrentRoom)
                ChangeChildAlpha(room.gameObject, 0);
            else
                ChangeChildAlpha(room.gameObject, 1);
        }
    }

    private void ChangeChildAlpha(GameObject go, float alpha, int childIndex = 0){
        GameObject child = go.transform.GetChild(childIndex).gameObject;

        SpriteRenderer sRenderer = child.GetComponent<SpriteRenderer>();

        if (sRenderer != null) {
            Color c = sRenderer.material.color;

            c.a = alpha;
            sRenderer.material.color = c;
        }

        int nextChildCount = child.transform.childCount;

        if (nextChildCount > 0){
            for(int i=0; i<nextChildCount; i++){
                ChangeChildAlpha(child, alpha, i);
            }
        }
        
    }

    public void GoToRoom(Room room)
    {
        CurrentRoom = room;
        MakeOnlyVisibleCurrentRoom();
    }
}

[CustomEditor(typeof(LayoutController))]
class LayoutControllerSceneEditor : Editor
{
    void OnSceneGUI()
    {
        LayoutController layout = target as LayoutController;

        foreach(Room room in layout.Map)
        {
            MapReference reference = room.transform.Find("MapReference").GetComponent<MapReference>();
            MapReferenceSceneEditor.DrawEdges(reference);
        }
    }
}