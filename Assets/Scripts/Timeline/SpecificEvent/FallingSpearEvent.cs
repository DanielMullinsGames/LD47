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

    [SerializeField]
    private Transform controlsHint;

    protected override void ResetToStart()
    {
        fallingSpear.gameObject.SetActive(false);
        fallingSpear.transform.position = startMarker.position;
        fallingSpear.GetComponent<SpriteRenderer>().sortingOrder = -1;
    }

    protected override void ResetToEnd()
    {
        fallingSpear.gameObject.SetActive(true);
        fallingSpear.transform.position = landMarker.position;
        fallingSpear.GetComponent<SpriteRenderer>().sortingOrder = 5;
    }

    protected override IEnumerator EventSequence()
    {
        bool learnedDucking = TutorialProgress.MechanicIsLearned(TutorialProgress.Mechanic.Ducking);

        fallingSpear.gameObject.SetActive(true);

        Tween.Position(fallingSpear, landMarker.position, learnedDucking ? 1f : 0.75f, 0f, Tween.EaseIn);
        yield return new WaitForSeconds(learnedDucking ? 0.65f : 0.5f);
        
        if (!PlayerController.Instance.Ducking)
        {
            Survived = false;
            PlayerController.Instance.Anim.SetTrigger("impale");
            PlayerController.Instance.Die();
            yield return new WaitForEndOfFrame();
            fallingSpear.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            yield break;
        }

        fallingSpear.GetComponent<SpriteRenderer>().sortingOrder = 5;
        yield return new WaitForSeconds(learnedDucking ? 0.35f : 0.25f);

        if (!learnedDucking)
        {
            Tween.Position(controlsHint.transform, controlsHint.transform.position + Vector3.down * 2f, 1f, 0f, Tween.EaseInOut);
            TutorialProgress.LearnMechanic(TutorialProgress.Mechanic.Ducking);
        }

        //embed in ground animation
    }
}