using UnityEngine;
using System.Collections;

public class AnimMethods : MonoBehaviour
{

    public bool AudioMuted { get { return audioMuted; } set { audioMuted = value; } }
    private bool audioMuted;

    void SendMessageUp(string message)
    {
        gameObject.SendMessageUpwards(message, SendMessageOptions.DontRequireReceiver);
    }

    void Destroy()
    {
        Destroy(transform.parent.gameObject);
    }

    void PlaySound(string soundId)
    {

    }
}
