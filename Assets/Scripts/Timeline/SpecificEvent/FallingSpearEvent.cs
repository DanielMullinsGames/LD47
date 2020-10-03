using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class FallingSpearEvent : TimelineEvent
{
    [SerializeField]
    private Transform fallingSpear;

    [SerializeField]
    private Transform startMarker;

    [SerializeField]
    private Transform landMarker;

    protected override IEnumerator EventSequence()
    {
        fallingSpear.transform.position = startMarker.position;
        Tween.Position(fallingSpear, landMarker.position, 0.75f, 0f, Tween.EaseIn);
        yield return new WaitForSeconds(0.75f);
        //If ducking: live
        //Else: die
    }
}