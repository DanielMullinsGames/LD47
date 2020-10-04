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
    private UnityEngine.Rendering.SortingGroup sortingGroup;

    [SerializeField]
    private float fallDuration;

    [SerializeField]
    private float durationUntilImpact;

    GameObject screamSound;

    protected override void ResetToStart()
    {
        fallingEnemy.gameObject.SetActive(false);
        fallingEnemy.transform.position = startMarker.position;
        fallingEnemyAnim.Play("falling", 0, 0f);
        sortingGroup.sortingOrder = 2;
        enemyDagger.GetComponent<SpriteRenderer>().enabled = true;
        GeneralReset();
    }

    protected override void ResetToEnd()
    {
        fallingEnemy.gameObject.SetActive(true);
        fallingEnemy.transform.position = corpseMarker.position;
        fallingEnemyAnim.Play("die", 0, 1f);
        sortingGroup.sortingOrder = 10;
        enemyDagger.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void GeneralReset()
    {
        Tween.Cancel(fallingEnemy.GetInstanceID());
        fallingEnemyAnim.ResetTrigger("die");

        if (screamSound != null)
        {
            Destroy(screamSound);
        }
    }

    protected override IEnumerator EventSequence()
    {
        screamSound = AudioController.Instance.PlaySound2D("enemy_scream").gameObject;
        yield return new WaitForSeconds(0.5f);
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
            yield return new WaitForSeconds((durationUntilImpact * durationUntilImpact) - timer);
            Tween.Stop(fallingEnemy.GetInstanceID());
            fallingEnemyAnim.SetTrigger("die");
            Destroy(screamSound);
            AudioController.Instance.PlaySound2D("enemy_scream_short");
            AudioController.Instance.PlaySound2D("misc_crunch_1", pitch: new AudioParams.Pitch(AudioParams.Pitch.Variation.Small));
            CameraEffects.Instance.Shake();
            sortingGroup.sortingOrder = 10;

            Vector2 landingPos = new Vector2(fallingEnemy.position.x, corpseMarker.position.y);
            Tween.Position(fallingEnemy.transform, landingPos, 0.15f, 0.1f, Tween.EaseIn);
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            enemyDagger.GetComponent<SpriteRenderer>().enabled = false;
            Survived = false;
            PlayerController.Instance.Anim.SetTrigger("knife");
            PlayerController.Instance.Die();
            yield return new WaitForSeconds(0.3f);
            yield break;
        }
    }
}