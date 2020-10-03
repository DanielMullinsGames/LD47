using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimelineEvent : MonoBehaviour
{
    public bool Survived { get; protected set; }

    [SerializeField]
    private float timeUntilDeath;

    public void SkipToEvent()
    {
        ResetEvent();
    }

    public IEnumerator PlayEvent()
    {
        Survived = false;
        yield return EventSequence();
    }

    protected abstract IEnumerator EventSequence();

    protected virtual void ResetEvent() { }
}