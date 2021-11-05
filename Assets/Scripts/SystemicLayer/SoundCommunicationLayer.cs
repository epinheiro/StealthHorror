using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCommunicationLayer : MonoBehaviour
{
    [SerializeField] SoundListener[] listeners;

    public static SoundCommunicationLayer instance;

    void Awake()
    {
        instance = this;
    }

    public void MakeSound(SoundType soundType, Vector3 position, Room room)
    {
        foreach(SoundListener listener in listeners)
        {
            listener.NewSound(soundType, position, room);
        }
    }
}
