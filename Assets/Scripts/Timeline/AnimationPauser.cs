using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPauser : Singleton<AnimationPauser>
{
    public bool Paused { get; private set; }

    [SerializeField]
    private List<Animator> animatorsToPause = default;

    [SerializeField]
    private List<MonoBehaviour> scriptsToPause = default;

    [SerializeField]
    private AudioSource pauseSound;

    public void SetPaused(bool paused)
    {
        pauseSound.gameObject.SetActive(paused);
        if (paused)
        {
            pauseSound.Stop();
            pauseSound.Play();
        }

        AudioController.Instance.SetLoopPaused(paused);

        Paused = paused;
        CameraEffects.Instance.ShowPaused(paused);
        animatorsToPause.ForEach(x => x.speed = paused ? 0f : 1f);
        scriptsToPause.ForEach(x => x.enabled = !paused);
    }
}