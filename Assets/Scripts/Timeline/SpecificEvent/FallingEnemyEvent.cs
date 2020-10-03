using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class FallingEnemyEvent : TimelineEvent
{
    [SerializeField]
    private Transform fallingEnemy;

    [SerializeField]
    private Animator fallingEnemyAnim;

    [SerializeField]
    private GameObject enemyDagger;

    [SerializeField]
    private Transform startMarker;

    [SerializeField]
    private Transform endMarker;

    [SerializeField]
    private Transform corpseMarker;

    [SerializeField]
    private float fallDuration;

    [SerializeField]
    private float durationUntilImpact;

    protected override void ResetToStart()
    {
        fallingEnemy.gameObject.SetActive(false);
        fallingEnemy.transform.position = startMarker.position;
        fallingEnemyAnim.Play("falling", 0, 0f);
        enemyDagger.SetActive(true);
    }

    protected override void ResetToEnd()
    {
        fallingEnemy.gameObject.SetActive(true);
        fallingEnemy.transform.position = corpseMarker.position;
        fallingEnemyAnim.Play("die", 0, 1f);
        enemyDagger.SetActive(true);
    }

    protected override IEnumerator EventSequence()
    {
        fallingEnemy.gameObject.SetActive(true);

        Tween.Position(fallingEnemy, endMarker.position, fallDuration, 0f, Tween.EaseIn);

        float timer = 0f;
        while (timer < durationUntilImpact) // check for enemy dead
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // if not enemy dead
        enemyDagger.SetActive(false);
        Survived = false;
        PlayerController.Instance.Anim.SetTrigger("knife");
        PlayerController.Instance.Die();
        yield return new WaitForSeconds(0.3f);
        yield break;
    }
}