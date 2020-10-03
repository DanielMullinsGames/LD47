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
        GeneralReset();
    }

    protected override void ResetToEnd()
    {
        fallingEnemy.gameObject.SetActive(true);
        fallingEnemy.transform.position = corpseMarker.position;
        fallingEnemyAnim.Play("die", 0, 1f);
    }

    private void GeneralReset()
    {
        Tween.Cancel(fallingEnemy.GetInstanceID());
        fallingEnemyAnim.ResetTrigger("die");
        enemyDagger.SetActive(true);
    }

    protected override IEnumerator EventSequence()
    {
        fallingEnemy.gameObject.SetActive(true);

        Tween.Position(fallingEnemy, endMarker.position, fallDuration, 0f, Tween.EaseIn);

        float timer = 0f;
        while (timer < durationUntilImpact && !PlayerController.Instance.Throwing)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (PlayerController.Instance.Throwing)
        {
            Tween.Cancel(fallingEnemy.GetInstanceID());
            fallingEnemyAnim.SetTrigger("die");

            Vector2 landingPos = new Vector2(fallingEnemy.position.x, corpseMarker.position.y);
            Tween.Position(fallingEnemy.transform, landingPos, 0.15f, 0.1f, Tween.EaseIn);
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            enemyDagger.SetActive(false);
            Survived = false;
            PlayerController.Instance.Anim.SetTrigger("knife");
            PlayerController.Instance.Die();
            yield return new WaitForSeconds(0.3f);
            yield break;
        }
    }
}