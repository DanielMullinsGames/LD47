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

    public SimpleEnemy CurrentEnemyTarget { get; set; }

    public bool Ducking => currentState == State.Ducking;
    public bool Dead => currentState == State.Dead;
    public Animator Anim => GetComponentInChildren<Animator>();

    public Weapon CurrentWeaponId { get; private set; }
    private GameObject CurrentWeapon => weapons[(int)CurrentWeaponId];
    [SerializeField]
    private List<GameObject> weapons;

    private bool releasedAttack;

    private State currentState = State.Standing;
    private Coroutine attackCooldownCoroutine;

    public void Reset()
    {
        CurrentEnemyTarget = null;
        SetState(State.Standing);
        Anim.Play("idle", 0, 0f);
        Anim.ResetTrigger("prepare_attack");
        Anim.ResetTrigger("end_attack");
        StopAllCoroutines();
    }

    public void Die()
    {
        SetState(State.Dead);
        if (attackCooldownCoroutine != null)
        {
            StopCoroutine(attackCooldownCoroutine);
        }
    }

    public void GainWeapon(Weapon weapon, bool immediate = true)
    {
        weapons.ForEach(x => x.SetActive(false));
        CurrentWeaponId = weapon;
        if (weapon != Weapon.None)
        {
            CurrentWeapon.SetActive(true);
        }
    }

    private void Attack()
    {
        Anim.Play("attack", 0, 0f);
        releasedAttack = true;
        CustomCoroutine.WaitThenExecute(0.05f, () => AttackConnect());
        attackCooldownCoroutine = StartCoroutine(AttackCooldown());
    }

    private void AttackConnect()
    {
        if (!Dead)
        {
            if (CurrentEnemyTarget != null)
            {
                GainWeapon(Weapon.None);
                CurrentEnemyTarget.Die();
            }
        }
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
                if (CurrentWeapon != null && Input.GetButtonDown("Attack"))
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