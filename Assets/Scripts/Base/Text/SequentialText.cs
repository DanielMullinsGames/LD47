using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SequentialText : MonoBehaviour
{
    public System.Action CharacterShown;

    public bool PlayingMessage { get; private set; }
    public bool EndOfVisibleCharacters { get; private set; }

    private TMPro.TextMeshPro textMesh;

    private float characterFrequency;

    private Color currentColor = Color.black;
    private bool skipToEnd;

    private const float DEFAULT_FREQUENCY = 10f;

    [SerializeField]
    private Color defaultColor = Color.black;

    [SerializeField]
    private string voiceSoundId = "dialogue_sound";

    public float voicePitch = 1f;

    void Awake()
    {
        textMesh = GetComponent<TMPro.TextMeshPro>();
        currentColor = defaultColor;
    }

    public void PlayMessage(string message)
    {
        SetToDefaults();

        if (TextActive())
        {
            StartCoroutine(PlayMessageSequence(message));
        }
    }

    public void Clear()
    {
        StopAllCoroutines();
        PlayingMessage = false;

        SetText("");
    }

    public void SkipToEnd()
    {
        skipToEnd = true;
    }

    public void SetText(string text)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
        }
    }

    protected bool TextActive()
    {
        return textMesh != null && textMesh.gameObject.activeInHierarchy;
    }

    protected string GetText()
    {
        return textMesh.text;
    }

    private void AppendText(string appendText)
    {
        SetText(GetText() + appendText);
    }

    private void SetToDefaults()
    {
        currentColor = defaultColor;
        characterFrequency = DEFAULT_FREQUENCY;
        skipToEnd = false;
    }

    private void FillTextBoxWithHiddenChars(string message)
    {
        for (int i = 0; i < message.Length; i++)
        {
            AppendText(ColorCharacter(message[i], new Color(0f, 0f, 0f, 0f)));
        }
    }

    private int RemoveFirstHiddenChar()
    {
        int startIndex = GetText().IndexOf("<color=#00000000>");
        SetText(GetText().Remove(startIndex, 26));
        return startIndex;
    }

    private IEnumerator PlayMessageSequence(string message)
    {
        PlayingMessage = true;
        EndOfVisibleCharacters = false;
        SetText("");

        string shown = "";

        FillTextBoxWithHiddenChars(message);

        int index = 0;
        while (message.Length > index)
        {
            if (message[index].ToString() == " ")
            {
                yield return new WaitForSeconds(0.025f);
            }
            else 
            {
                CharacterShown?.Invoke();
                PlaySound();
            }
            shown += message[index];
            if (message == shown)
            {
                EndOfVisibleCharacters = true;
            }

            int insertIndex = RemoveFirstHiddenChar();
            string newCharacterWithColorCode = ColorCharacter(message[index], currentColor);
            SetText(GetText().Insert(insertIndex, newCharacterWithColorCode));
            index++;

            if (index > 0 && (message[index-1] == '.' || message[index-1] == '?' || message[index-1] == '!'))
            {
                yield return new WaitForSeconds(Skipping() ? 0.03f : 0.3f);
            }

            float adjustedFrequency = Mathf.Clamp(characterFrequency * 0.01f, 0.01f, 0.2f);

            if (Skipping())
            {
                adjustedFrequency *= 0.1f;
            }

            float waitTimer = 0f;
            while (!skipToEnd && waitTimer < adjustedFrequency)
            {
                waitTimer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForSeconds(0.15f);
        PlayingMessage = false;
    }

    private bool Skipping()
    {
        return Input.GetButton("Duck") || Input.GetButton("Attack") || Input.GetButton("Throw");
    }

    private void PlaySound()
    {
        AudioController.Instance.PlaySound2D(voiceSoundId, volume: 1f, pitch: new AudioParams.Pitch(AudioParams.Pitch.Variation.VerySmall), 
            randomization: new AudioParams.Randomization(), 
            repetition: new AudioParams.Repetition(0.1f, "dialogue"));
    }

    public static string ColorString(string str, Color c)
    {
        string coloredString = "<color=#" + ColorUtility.ToHtmlStringRGBA(c) + ">";
        coloredString += str;
        coloredString += "</color>";

        return coloredString;
    }

    public static string ColorCharacter(char character, Color c)
    {
        return ColorString(character.ToString(), c);
    }
}