using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class FlyingArrowEvent : TimelineEvent
{
    [SerializeField]
    private Transform arrow;

    [SerializeField]
    private Transform startMarker;

    [SerializeField]
    private Transform endMarker;

    protected override void ResetToStart()
    {
        arrow.gameObject.SetActive(false);
        arrow.transform.position = startMarker.position;
    }

    protected override void ResetToEnd()
    {
        arrow.gameObject.SetActive(false);
    }

    protected override IEnumerator EventSequence()
    {
        AudioController.Instance.PlaySound2D("twang", MixerGroup.None, volume: 0.25f, pitch: new AudioParams.Pitch(1.5f));
        arrow.gameObject.SetActive(true);

        Tween.Position(arrow, endMarker.position, 2f, 0f, Tween.EaseIn);
        
        yield return new WaitForSeconds(1.1f);
        if (!PlayerController.Instance.Ducking)
        {
            Survived = false;
            PlayerController.Instance.Anim.SetTrigger("arrow");
            PlayerController.Instance.Die();
            yield return new WaitForEndOfFrame();
            arrow.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            yield break;
        }

        yield return new WaitForSeconds(0.25f);
    }
}