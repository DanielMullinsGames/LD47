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
        PreparingAttack,
    }

    public enum Weapon
    {
        None,
        Spear,
        Knife,
    }

    public bool Ducking => currentState == State.Ducking;
    public bool Dead => currentState == State.Dead;
    public Animator Anim => GetComponentInChildren<Animator>();

    [SerializeField]
    private List<GameObject> weapons;

    private GameObject currentWeapon;
    private bool releasedAttack;

    private State currentState = State.Standing;
    private Coroutine attackCooldownCoroutine;

    public void Reset()
    {
        SetState(State.Standing);
        Anim.Play("idle", 0, 0f);
        Anim.ResetTrigger("prepare_attack");
        Anim.ResetTrigger("end_attack");
        StopAllCoroutines();
    }

    public void Die()
    {
        SetState(State.Dead);
        StopCoroutine(attackCooldownCoroutine);
    }

    public void GainWeapon(Weapon weapon, bool immediate = true)
    {
        if (weapon == Weapon.None)
        {
            currentWeapon = null;
        }
        else
        {
            currentWeapon = weapons[(int)weapon];
            weapons.ForEach(x => x.SetActive(false));
            currentWeapon.SetActive(true);
        }
    }

    private void Attack()
    {
        Anim.Play("attack", 0, 0f);
        releasedAttack = true;
        attackCooldownCoroutine = StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(0.25f);
        Anim.SetTrigger("end_attack");
        SetState(State.Standing);
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

        if (newState == State.PreparingAttack)
        {
            Anim.SetTrigger("prepare_attack");
            releasedAttack = false;
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
                if (currentWeapon != null && Input.GetButtonDown("Attack"))
                {
                    SetState(State.PreparingAttack);
                }
                break;
            case State.Ducking:
                if (Input.GetButtonUp("Duck"))
                {
                    SetState(State.Standing);
                }
                break;
            case State.PreparingAttack:
                if (!releasedAttack && Input.GetButtonUp("Attack"))
                {
                    Attack();
                }
                break;
        }
    }
}