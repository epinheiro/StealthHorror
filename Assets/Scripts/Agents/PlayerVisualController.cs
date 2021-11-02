using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    string prefix = "Player";

    enum Type
    {
        Idle,
        Walking,
        Running
    }

    enum Action
    {
        Movement,
        Interact,
        Falling
    }

    enum Direction
    {
        Down,
        Left,
        Up,
        Right
    }

    // AnimationController controller;

    // void Awake()
    // {
    //     this.GetComponent<Controller>
    // }

    // Update is called once per frame
    void Update()
    {
        
    }
}
