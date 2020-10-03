using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimelineEvent : MonoBehaviour
{
    public bool Survived { get; protected set; }

    public float EventProgress => Mathf.Clamp((Time.time - timeStarted) / timeUntilDeath, 0f, 1f);

    [SerializeField]
    private float timeUntilDeath = default;

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