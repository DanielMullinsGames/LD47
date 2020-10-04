using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEvent : TimelineEvent
{
    [SerializeField]
    private List<DialogueBubble> dialogueBubbles;

    protected override IEnumerator EventSequence()
    {
        PlayerController.Instance.enabled = false;

        foreach (DialogueBubble b in dialogueBubbles)
        {
            b.gameObject.SetActive(true);
            yield return b.PlayDialogue();
            yield return new WaitForSeconds(0.1f);
            b.gameObject.SetActive(false);
        }

        PlayerController.Instance.enabled = true;
    }
}