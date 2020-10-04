using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBubble : MonoBehaviour
{
    [TextArea]
    [SerializeField]
    private string message;

    [SerializeField]
    private SequentialText text;

    [SerializeField]
    private Animator anim;

    public IEnumerator PlayDialogue()
    {
        anim.Play("open");
        text.Clear();
        yield return new WaitForSeconds(0.15f);
        text.PlayMessage(message);
        yield return new WaitWhile(() => text.PlayingMessage);
        yield return new WaitForSeconds(0.25f);
        anim.Play("close");
    }
}