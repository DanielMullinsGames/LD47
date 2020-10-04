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

    public PlayerController.Weapon startWeapon;

    private float timeStarted;
    private bool playing;

    public void ResetEventToStart()
    {
        Survived = true;
        playing = false;
        PlayerController.Instance.GainWeapon(startWeapon);
        PlayerController.Instance.CurrentEnemyTarget = null;
        ResetToStart();
    }

    public void ResetEventToEnd()
    {
        Survived = true;
        playing = false;
        ResetToEnd();
    }

    public IEnumerator PlayEvent()
    {
        ResetEventToStart();
        timeStarted = Time.time;
        playing = true;
        yield return EventSequence();
        playing = false;
    }

    protected abstract IEnumerator EventSequence();

    protected virtual void ResetToStart() { }
    protected virtual void ResetToEnd() { }
}