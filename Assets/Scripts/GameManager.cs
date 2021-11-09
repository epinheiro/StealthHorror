using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public DebugSettings DebugSettings;

    public PlayerController Player {get; protected set;}
    public MonsterController Monster {get; protected set;}

    void Awake()
    {
        Instance = this;
        DebugSettings = Resources.Load<DebugSettings>("DebugSettings");
        Player = GameObject.Find("Player").GetComponent<PlayerController>();
        Monster = GameObject.Find("Monster").GetComponent<MonsterController>();
    }
}
