using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    ///////////// Meta values /////////////
    float simpleModifier = 1;
    float runningModifier = 1.75f;

    ///////////// Control references /////////////
    [SerializeField] LayoutController map;
    Room currentRoom;
    Room adjacentRoom;
    bool canMakeTransition;


    // Start is called before the first frame update
    void Awake()
    {
        currentRoom = map.CurrentRoom;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 movVector = new Vector2(x, y);

        Vector3 nextPositon = this.transform.position + movVector * simpleModifier * (Input.GetButton("Fire3") ? runningModifier : 1) * Time.deltaTime;

        if(currentRoom.IsPointInsideConvexPolygon(nextPositon))
        {
            this.transform.position = nextPositon;
            adjacentRoom = currentRoom.IsPointInRoomTransition(nextPositon);
            canMakeTransition = adjacentRoom != null;
            // if(canMakeTransition) Debug.LogError($"IsPointInRoomTransition [{currentRoom.IsPointInRoomTransition(nextPositon).name}]");
            if(canMakeTransition && Input.GetButtonDown("Jump"))
            {
                map.GoToRoom(adjacentRoom);
                currentRoom = adjacentRoom;
            }
        }
    }
}
