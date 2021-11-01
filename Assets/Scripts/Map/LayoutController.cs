using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutController : MonoBehaviour
{
    public Room CurrentRoom;
    [SerializeField] public List<Room> Map;

    void Awake()
    {
        foreach(Room room in Map)
        {
            if(room != CurrentRoom)
                ChangeChildAlpha(room.gameObject, 0);
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
}
