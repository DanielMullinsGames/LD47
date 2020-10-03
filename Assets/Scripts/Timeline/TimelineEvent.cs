using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimelineEvent : MonoBehaviour
{
    public bool Survived { get; protected set; }

    public float EventProgress => playing ? Mathf.Clamp((Time.time - timeStarted) / length, 0f, 1f) : 0f;

    public float Length => length;
    [SerializeField]
    private float length = default;

    private float timeStarted;
    private bool playing = true;

    public void ResetEventToStart()
    {
        ResetToStart();
    }

    public void ResetEventToEnd()
    {
        ResetToEnd();
    }

    public IEnumerator PlayEvent()
    {
        timeStarted = Time.time;
        playing = true;
        Survived = true;
        ResetToStart();
        yield return EventSequence();
        playing = false;
    }

    protected abstract IEnumerator EventSequence();

    protected virtual void ResetToStart() { }
    protected virtual void ResetToEnd() { }
}