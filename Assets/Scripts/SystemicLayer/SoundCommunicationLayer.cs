using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCommunicationLayer : MonoBehaviour
{
    List<SoundListener> listeners;

    public static SoundCommunicationLayer instance;

    void Awake()
    {
        instance = this;

        GameObject[] goArray = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach(GameObject go in goArray)
        {
            SoundListener listener = go.GetComponent<SoundListener>();
            if(listener != null)
                listeners.Add(listener);
        }
    }

    public void MakeSound(SoundType soundType, Vector3 position, Room room)
    {
        foreach(SoundListener listener in listeners)
        {
            listener.NewSound(soundType, position, room);
        }
    }
}
