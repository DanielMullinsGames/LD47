using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }
    }
}