using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class EnemyEvent : TimelineEvent
{
    [SerializeField]
    private SimpleEnemy enemy;

    [SerializeField]
    private Transform startMarker;

    [SerializeField]
    private Transform endMarker;

    [SerializeField]
    private float telegraphTime = 0.75f;

    [SerializeField]
    private string playerDeathAnimation = "knife";

    [SerializeField]
    private Transform controlsHint;

    protected override void ResetToStart()
    {
        enemy.transform.position = new Vector2(startMarker.position.x, enemy.transform.position.y);
        enemy.ResetToStart();
    }

    protected override void ResetToEnd()
    {
        enemy.transform.position = new Vector2(endMarker.position.x, enemy.transform.position.y);
        enemy.ResetToEnd();
    }

    protected override IEnumerator EventSequence()
    {
        if (!TutorialProgress.MechanicIsLearned(TutorialProgress.Mechanic.Attacking) && controlsHint != null)
        {
            Tween.Stop(controlsHint.GetInstanceID());
            controlsHint.transform.localPosition = new Vector3(0f, -6.31f, 0f);
            Tween.Position(controlsHint.transform, controlsHint.transform.position + Vector3.up * 2f, 0.25f, 0f, Tween.EaseInOut);
        }

        enemy.MoveToTarget(endMarker.position.x);
        yield return new WaitUntil(() => enemy.Dead || enemy.ReachedTarget);

        if (!enemy.Dead)
        {
            PlayerController.Instance.CurrentEnemyTarget = enemy;
            enemy.RaiseWeapon();
            float strikeTimer = 0f;
            while (strikeTimer < telegraphTime && !enemy.Dead)
            {
                strikeTimer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (controlsHint != null)
            {
                Tween.Position(controlsHint.transform, controlsHint.transform.position + Vector3.up * -3f, 0.25f, 0f, Tween.EaseInOut);
            }

            if (!enemy.Dead)
            {
                enemy.Strike();
                yield return new WaitForSeconds(0.1f);
                if (!enemy.Dead)
                {
                    Survived = false;
                    PlayerController.Instance.Anim.SetTrigger(playerDeathAnimation);
                    PlayerController.Instance.Die();
                    yield return new WaitForSeconds(0.3f);
                    yield break;
                }
            }

            yield return new WaitForSeconds(0.25f);
            TutorialProgress.LearnMechanic(TutorialProgress.Mechanic.Attacking);
        }
    }
}