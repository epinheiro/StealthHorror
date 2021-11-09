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
    public MapGraph Graph;

    void Awake()
    {
        MakeOnlyVisibleCurrentRoom();
        GameManager.Instance.Player.ChangingRoom += PlayerChangingRoom;
        GameManager.Instance.Monster.ChangingRoom += MonsterChangingRoom;
    }

    void Start()
    {
        Graph = new MapGraph(Rooms);
    }

    private void MakeOnlyVisibleCurrentRoom()
    {
        foreach (Room room in Rooms)
        {
            if( GameManager.Instance.DebugSettings.AlwaysShowSprites )
            {
                ChangeChildAlpha(room.gameObject, 1);
            }
            else
            {
                if (room != PlayerCurrentRoom)
                    ChangeChildAlpha(room.gameObject, 0);
                else
                    ChangeChildAlpha(room.gameObject, 1);
            }
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

    public void PlayerChangingRoom(Room room)
    {
        PlayerCurrentRoom = room;
        MakeOnlyVisibleCurrentRoom();
    }

    public void MonsterChangingRoom(Room room)
    {
        MonsterCurrentRoom = room;
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