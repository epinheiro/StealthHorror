using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public DebugSettings DebugSettings;

    void Awake()
    {
        Instance = this;
        DebugSettings = Resources.Load<DebugSettings>("DebugSettings");
    }
}
