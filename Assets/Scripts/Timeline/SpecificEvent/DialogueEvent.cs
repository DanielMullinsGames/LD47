using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class DialogueEvent : TimelineEvent
{
    [SerializeField]
    private List<DialogueBubble> dialogueBubbles;

    [SerializeField]
    private Transform liveComrade;
    private Vector2 comradeStartPos;

    [SerializeField]
    private GameObject skipHint;

    [SerializeField]
    private bool finale;

    [SerializeField]
    private Animator wife;

    [SerializeField]
    private GameObject wifeMedallion;

    [SerializeField]
    private Transform fallingSpear;

    [SerializeField]
    private Transform startMarker;

    [SerializeField]
    private Transform landMarker;

    [SerializeField]
    private GameObject campAudio;

    private bool returned;

    private void Awake()
    {
        if (liveComrade != null)
        {
            comradeStartPos = liveComrade.position;
        }
    }

    protected override void ResetToStart()
    {
        if (liveComrade != null)
        {
            liveComrade.position = comradeStartPos;
            liveComrade.gameObject.SetActive(true);
            liveComrade.GetComponentInChildren<Animator>().Play("idle", 0, 0f);
        }
    }

    protected override void ResetToEnd()
    {
        if (liveComrade != null)
        {
            liveComrade.position = comradeStartPos;
            liveComrade.gameObject.SetActive(false);
        }
    }

    protected override IEnumerator EventSequence()
    {
        if (campAudio != null)
        {
            CustomCoroutine.WaitThenExecute(2f, () =>
            {
                campAudio.SetActive(true);
                AudioController.Instance.FadeOutLoop(2f);
            });
        }
        if (skipHint != null && returned)
        {
            skipHint.SetActive(true);
        }

        PlayerController.Instance.enabled = false;
        yield return new WaitForSeconds(0.2f);
        foreach (DialogueBubble b in dialogueBubbles)
        {
            b.gameObject.SetActive(true);
            yield return b.PlayDialogue();
            yield return new WaitForSeconds(0.1f);
            b.gameObject.SetActive(false);
        }

        if (liveComrade != null)
        {
            liveComrade.GetComponentInChildren<Animator>().SetTrigger("slide");
            Tween.Position(liveComrade, liveComrade.transform.position + (Vector3.right * 12f), 0.5f, 0f, Tween.EaseInOut);
            yield return new WaitForSeconds(0.5f);
            liveComrade.gameObject.SetActive(false);
        }

        if (finale)
        {
            yield return Finale();
        }
        else
        {
            PlayerController.Instance.enabled = true;
        }
        returned = true;
    }

    private IEnumerator Finale()
    {
        yield return new WaitForSeconds(0.5f);
        PlayerController.Instance.HideMedallion();
        wife.SetTrigger("gain_medallion");
        wifeMedallion.SetActive(true);
        AudioController.Instance.PlaySound2D("misc_crunch_1");

        yield return new WaitForSeconds(2f);

        Tween.Position(fallingSpear, landMarker.position, 0.75f, 0f, Tween.EaseIn);
        yield return new WaitForSeconds(0.5f);

        PlayerController.Instance.Anim.SetTrigger("impale");
        PlayerController.Instance.Die();
        yield return new WaitForEndOfFrame();
        fallingSpear.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.4f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("end");
    }
}