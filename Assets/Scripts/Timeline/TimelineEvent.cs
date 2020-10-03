using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimelineEvent : MonoBehaviour
{
    public bool Survived { get; protected set; }

    public float EventProgress => Mathf.Clamp((Time.time - timeStarted) / length, 0f, 1f);

    public float Length => length;
    [SerializeField]
    private float length = default;

    private float timeStarted;

    public void SkipToEvent()
    {
        ResetEvent();
    }

    public IEnumerator PlayEvent()
    {
        timeStarted = Time.time;
        Survived = true;
        yield return EventSequence();
    }

    protected abstract IEnumerator EventSequence();

    protected virtual void ResetEvent() { }
}