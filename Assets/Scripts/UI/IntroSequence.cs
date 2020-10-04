using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class IntroSequence : MonoBehaviour
{
    public bool skipIntro;

    [SerializeField]
    private Transform cam;

    private void Start()
    {
#if !UNITY_EDITOR
        skipIntro = false;
#endif
        if (!skipIntro)
        {
            StartCoroutine(Intro());
        }
    }

    public IEnumerator Intro()
    {
        TimelineBar.Instance.transform.position = new Vector3(0f, 3f, 10f);
        cam.transform.position = new Vector3(0f, 15f, cam.transform.position.z);
        GetComponent<Animator>().enabled = true;
        StartCoroutine(FadeInVolume(20f));
        yield return new WaitForSeconds(11f);
        Tween.Position(cam, new Vector3(0f, 0f, cam.transform.position.z), 9f, 0f, Tween.EaseInOut);
        yield return new WaitForSeconds(9f);
        Tween.LocalPosition(TimelineBar.Instance.transform, new Vector3(0f, 0f, 10f), 0.5f, 0f, Tween.EaseInOut);
    }

    public IEnumerator FadeInVolume(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            AudioListener.volume = timer / duration;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        AudioListener.volume = 1f;
    }
}