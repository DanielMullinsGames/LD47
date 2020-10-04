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
        PreparingThrow,
        Moving,
    }

    public enum Weapon
    {
        None,
        Spear,
        Knife,
    }

    public SimpleEnemy CurrentEnemyTarget { get; set; }

    public bool Moving => currentState == State.Moving;
    public bool Throwing { get; private set; }
    public bool Ducking => currentState == State.Ducking;
    public bool Dead => currentState == State.Dead;
    public Animator Anim => GetComponentInChildren<Animator>();

    public Weapon CurrentWeaponId { get; private set; }
    private GameObject CurrentWeapon => weapons[(int)CurrentWeaponId];
    [SerializeField]
    private List<GameObject> weapons;

    private bool releasedAttack;
    private bool releasedThrow;

    private State currentState = State.Standing;
    private Coroutine attackCooldownCoroutine;
    private Coroutine throwCooldownCoroutine;

    public void Reset()
    {
        CurrentEnemyTarget = null;
        Throwing = false;
        SetState(State.Standing);
        Anim.Play("idle", 0, 0f);
        Anim.ResetTrigger("prepare_attack");
        Anim.ResetTrigger("end_attack");
        Anim.ResetTrigger("prepare_throw");
        Anim.ResetTrigger("end_throw");
        StopAllCoroutines();
    }

    public void StartMove()
    {
        SetState(State.Moving);
    }

    public void EndMove()
    {
        SetState(State.Standing);
    }

    public void Die()
    {
        AudioController.Instance.PlaySound2D("misc_crunch_1");
        SetState(State.Dead);
        if (attackCooldownCoroutine != null)
        {
            StopCoroutine(attackCooldownCoroutine);
        }
        if (throwCooldownCoroutine != null)
        {
            StopCoroutine(throwCooldownCoroutine);
        }
        Throwing = false;
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

    private void Throw()
    {
        Anim.Play("throw", 0, 0f);
        releasedThrow = true;
        throwCooldownCoroutine = StartCoroutine(ThrowCooldown());
    }

    private IEnumerator ThrowCooldown()
    {
        Throwing = true;
        yield return new WaitForSeconds(0.25f);
        Throwing = false;
        Anim.SetTrigger("end_throw");
        GainWeapon(Weapon.None);
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

        if (newState == State.PreparingThrow)
        {
            Anim.SetTrigger("prepare_throw");
            releasedThrow = false;
        }

        if (newState == State.Moving)
        {
            Anim.SetBool("moving", true);
        }
        else
        {
            Anim.SetBool("moving", false);
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
                if (CurrentWeaponId != Weapon.None)
                {
                    if (Input.GetButtonDown("Attack"))
                    {
                        SetState(State.PreparingAttack);
                    }
                    else if (Input.GetButtonDown("Throw"))
                    {
                        SetState(State.PreparingThrow);
                    }
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
            case State.PreparingThrow:
                if (!releasedThrow && Input.GetButtonUp("Throw"))
                {
                    Throw();
                }
                break;
        }
    }
}