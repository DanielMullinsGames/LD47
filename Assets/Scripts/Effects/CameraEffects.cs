using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : Singleton<CameraEffects>
{
    [SerializeField]
    private Animator uiCamAnim;

	public void ShowRewind()
    {
        uiCamAnim.Play("rewind", 0, 0f);
    }
}