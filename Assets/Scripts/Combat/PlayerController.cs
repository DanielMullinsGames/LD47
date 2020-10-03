using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public bool Ducking => false;

    public Animator Anim => GetComponentInChildren<Animator>();
}