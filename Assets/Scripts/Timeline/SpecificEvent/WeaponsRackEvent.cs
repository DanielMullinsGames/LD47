using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsRackEvent : TimelineEvent
{
    [SerializeField]
    private SpriteRenderer weapon;

    protected override void ResetToStart()
    {
        weapon.enabled = true;
    }

    protected override void ResetToEnd()
    {
        weapon.enabled = false;
    }

    protected override IEnumerator EventSequence()
    {
        yield return new WaitForSeconds(0.25f);
        // cute animation
        yield return new WaitForSeconds(0.5f);
    }
}