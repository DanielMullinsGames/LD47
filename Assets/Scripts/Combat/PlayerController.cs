using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public enum State
    {
        Standing,
        Ducking,
        Dead,
    }

    public bool Ducking => currentState == State.Ducking;
    public bool Dead => currentState == State.Dead;
    public Animator Anim => GetComponentInChildren<Animator>();

    private State currentState = State.Standing;

    public void Reset()
    {
        SetState(State.Standing);
        Anim.Play("idle", 0, 0f);
    }

    public void Die()
    {
        SetState(State.Dead);
    }

    private void SetState(State newState)
    {
        if (newState == State.Ducking)
        {
            Anim.Play("ducking", 0, 0f);
            Anim.SetBool("ducking", true);
        }
        else
        {
            Anim.SetBool("ducking", false);
        }
        currentState = newState;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Standing:
                if (Input.GetButtonDown("Duck"))
                {
                    SetState(State.Ducking);
                }
                break;
            case State.Ducking:
                if (Input.GetButtonUp("Duck"))
                {
                    SetState(State.Standing);
                }
                break;
        }
    }
}