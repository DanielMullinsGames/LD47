using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class CameraEffects : Singleton<CameraEffects>
{
    [SerializeField]
    private Animator uiCamAnim;

    [SerializeField]
    private UnityStandardAssets.ImageEffects.Grayscale grayScaleEffect;

    public void Shake()
    {
        GetComponent<Animation>().Play();
    }

	public void ShowRewind()
    {
        uiCamAnim.Play("rewind", 0, 0f);
    }

    public void ShowPaused(bool paused)
    {
        grayScaleEffect.enabled = paused;
        uiCamAnim.Play(paused ? "paused" : "idle", 0, 0f);
    }
}