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

        PlayerController.Instance.enabled = true;
    }
}