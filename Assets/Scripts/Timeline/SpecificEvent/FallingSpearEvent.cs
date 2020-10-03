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
        fallingSpear.gameObject.SetActive(true);
        fallingSpear.transform.position = startMarker.position;
        Tween.Position(fallingSpear, landMarker.position, 0.75f, 0f, Tween.EaseIn);
        yield return new WaitForSeconds(0.5f);
        
        if (!PlayerController.Instance.Ducking)
        {
            // impale animation
            Survived = false;
            PlayerController.Instance.Anim.SetTrigger("impale");
            PlayerController.Instance.Die();
            yield return new WaitForEndOfFrame();
            fallingSpear.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            yield break;
        }

        fallingSpear.GetComponent<SpriteRenderer>().sortingOrder = 5;
        yield return new WaitForSeconds(0.25f);

        //embed in ground animation
    }
}