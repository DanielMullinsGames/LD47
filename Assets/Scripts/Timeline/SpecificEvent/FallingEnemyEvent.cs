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
    private GameObject signal;

    [SerializeField]
    private Transform signalStart;

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

    [SerializeField]
    private Transform controlsHint;

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
        if (!TutorialProgress.MechanicIsLearned(TutorialProgress.Mechanic.Throwing) && controlsHint != null)
        {
            Tween.Stop(controlsHint.GetInstanceID());
            controlsHint.transform.localPosition = new Vector3(0f, -6.31f, 0f);
            Tween.Position(controlsHint.transform, controlsHint.transform.position + Vector3.up * 2f, 0.25f, 0f, Tween.EaseInOut);
        }

        screamSound = AudioController.Instance.PlaySound2D("enemy_scream").gameObject;
        yield return new WaitForSeconds(0.2f);

        signal.SetActive(true);
        signal.transform.position = signalStart.position;
        Tween.Position(signal.transform, signalStart.position + new Vector3(-2f, 5f), 0.5f, 0f, Tween.EaseInOut);

        yield return new WaitForSeconds(0.5f);
        signal.SetActive(false);
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
            if (controlsHint != null)
            {
                Tween.Position(controlsHint.transform, controlsHint.transform.position + Vector3.up * -3f, 0.25f, 0f, Tween.EaseInOut);
            }
            TutorialProgress.LearnMechanic(TutorialProgress.Mechanic.Throwing);

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

            if (controlsHint != null)
            {
                Tween.Position(controlsHint.transform, controlsHint.transform.position + Vector3.up * -3f, 0.25f, 0f, Tween.EaseInOut);
            }
        }
    }
}