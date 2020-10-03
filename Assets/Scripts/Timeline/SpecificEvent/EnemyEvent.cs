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
            // raise weapon
            // wait to see if dead
            // if lived, kill player + reset
        }
    }
}