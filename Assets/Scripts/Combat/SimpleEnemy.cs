﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    public bool ReachedTarget { get; private set; }

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private float speed;

    private bool hasTarget;
    private float targetX;

    public bool Dead { get; private set; }

	public void ResetToStart()
    {
        Dead = false;
        anim.Play("running", 0, 0f);
        hasTarget = false;
    }

    public void ResetToEnd()
    {
        anim.Play("die", 0, 1f);
    }

    public void Die()
    {
        Dead = true;
        anim.Play("die", 0, 0f);
    }

    public void MoveToTarget(float xPos)
    {
        targetX = xPos;
        hasTarget = true;
        ReachedTarget = false;
    }

    public void RaiseWeapon()
    {
        anim.SetTrigger("raise_weapon");
    }

    public void Strike()
    {
        anim.Play("attack", 0, 0f);
    }

    private void Update()
    {
        if (!Dead && hasTarget)
        {
            UpdateMoveToTarget();
        }
    }

    private void UpdateMoveToTarget()
    {
        if (targetX < transform.position.x)
        {
            transform.position += new Vector3(Time.deltaTime * speed, 0f, 0f);

            if (transform.position.x < targetX)
            {
                ReachedTarget = true;
                transform.position = new Vector2(targetX, transform.position.y);
            }
        }
    }
}