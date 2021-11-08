using System;
using UnityEngine;

public class SoundListener : MonoBehaviour
{
    public Action<SoundType, Vector3, Room> SoundEvent; 

    void Start()
    {
        SoundCommunicationLayer.instance.SignListener(this);
    }

    public void NewSound(SoundType soundType, Vector3 position, Room room)
    {
        SoundEvent?.Invoke(soundType, position, room);
    }
}
