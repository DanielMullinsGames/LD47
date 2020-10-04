using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SimpleEnemy : MonoBehaviour
{
    public bool ReachedTarget { get; private set; }

    public bool playerKeepsWeapon;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private float speed;

    [SerializeField]
    private SortingGroup sortingGroup;

    [SerializeField]
    private SpriteRenderer weaponSprite;

    private bool hasTarget;
    private float targetX;

    public bool Dead { get; private set; }

	public void ResetToStart()
    {
        sortingGroup.sortingOrder = 2;
        Dead = false;
        anim.Play("running", 0, 0f);
        hasTarget = false;
        weaponSprite.enabled = true;
    }

    public void ResetToEnd()
    {
        anim.Play("die", 0, 1f);
        sortingGroup.sortingOrder = 10;
        weaponSprite.enabled = false;
    }

    public void Die()
    {
        AudioController.Instance.PlaySound2D("misc_crunch_1", pitch: new AudioParams.Pitch(AudioParams.Pitch.Variation.Small));
        Dead = true;
        anim.Play("die", 0, 0f);
        sortingGroup.sortingOrder = 10;
        CameraEffects.Instance.Shake();
    }

    public void MoveToTarget(float xPos)
    {
        AudioController.Instance.PlaySound2D("enemy_scream_short", pitch: new AudioParams.Pitch(AudioParams.Pitch.Variation.Small));
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