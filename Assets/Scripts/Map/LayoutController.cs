using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LayoutController : MonoBehaviour
{
    public bool ShowGraphOnEditor = false;

    public Room PlayerCurrentRoom;
    public Room MonsterCurrentRoom;
    [SerializeField] public List<Room> Rooms;
    MapGraph map;

    void Awake()
    {
        MakeOnlyVisibleCurrentRoom();
    }

    void Start()
    {
        map = new MapGraph(Rooms);
    }

    private void MakeOnlyVisibleCurrentRoom()
    {
        foreach (Room room in Rooms)
        {
            if (room != PlayerCurrentRoom)
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
        PlayerCurrentRoom = room;
        MakeOnlyVisibleCurrentRoom();
    }
}

[CustomEditor(typeof(LayoutController))]
class LayoutControllerSceneEditor : Editor
{
    void OnSceneGUI()
    {
        LayoutController layout = target as LayoutController;

        if(layout.ShowGraphOnEditor)
        {
            foreach(Room room in layout.Rooms)
            {
                MapReference reference = room.transform.Find("MapReference").GetComponent<MapReference>();
                MapReferenceSceneEditor.DrawEdges(reference);
            }
        }
    }
}