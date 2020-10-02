using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatingSprite : MonoBehaviour
{
    public bool Reversed { get; set; }
    public float AnimSpeed { get { return animSpeed; } set { animSpeed = value; } }

    [Header("Frames")]
	public List<Sprite> frames = new List<Sprite>();
    public List<Texture2D> textureFrames = new List<Texture2D>();

    [SerializeField]
    private string textureField = "_MainTex";

    [SerializeField]
    private bool randomizeSprite = default;

    [SerializeField]
    private bool noRepeat = default;

    [SerializeField]
    private bool setFirstFrameOnAwake = true;

    [Header("Animation")]
    [SerializeField]
    private float animSpeed = 0.033f;

    [SerializeField]
    private float animOffset = 0f;

    [SerializeField]
    private int frameOffset = 0;

    [SerializeField]
    private bool randomOffset = default;

    [SerializeField]
	private bool stopAfterSingleIteration = false;

    [SerializeField]
    private bool pingpong = default;

    [Header("Audio")]
    [SerializeField]
    private string soundId = default;

    [SerializeField]
    private int soundFrame = default;

    [Header("Blinking")]
    public List<Sprite> blinkFrames = new List<Sprite>();
    public float blinkRate;
    public float doubleBlinkRate;

    int blinkFrameIndex;
    float blinkTimer;

    float timer;
	SpriteRenderer sR;
    Renderer r;
    private int frameCount;

	[HideInInspector]
	public int frameIndex = 0;

	private bool stopOnNextFrame;

    void Awake()
    {
        sR = GetComponent<SpriteRenderer>();
        r = GetComponent<Renderer>();

        UpdateFrameCount();
        if (frameOffset > 0)
        {
            frameIndex = frameOffset;
        }

        if (setFirstFrameOnAwake)
        {
            if (frameCount > 0)
            {
                SetFrame(frameIndex);
            }
        }
    }

	void Start()
    {
        if (randomOffset)
        {
            animOffset = -Random.value * (frameCount * animSpeed);
        }
		timer = animOffset;
	}

	public void StartAnimatingWithDecrementedIndex()
    {
		frameIndex--;
		StartAnimating ();
	}

	public void StartAnimating()
    {
		this.enabled = true;
		stopOnNextFrame = false;
	}

	public void StopAnimating()
    {
		stopOnNextFrame = true;
	}

    public void StopImmediate()
    {
        Stop();
    }

	public void StartFromBeginning()
    {
		this.enabled = true;
		frameIndex = 0;
        SetFrame(0);
        timer = 0f;
    }

    public void SkipToEnd()
    {
        if (Reversed)
        {
            frameIndex = 0;
        }
        else
        {
            frameIndex = frames.Count - 1;
        }
        SetFrame(frameIndex);
    }

    public void IterateFrame()
    {
        if (stopOnNextFrame)
        {
            Stop();
            return;
        }

        timer = 0f;

        if (blinkFrames.Count > 0)
        {
            if (blinkTimer > blinkRate)
            {
                sR.sprite = blinkFrames[blinkFrameIndex];

                if (blinkFrameIndex < blinkFrames.Count -1)
                {
                    blinkFrameIndex++;
                }
                else
                {
                    if (Random.value < doubleBlinkRate)
                    {
                        blinkTimer = blinkRate - 0.1f;
                    }
                    else
                    {
                        blinkTimer = Random.value * -0.5f;
                    }
                    blinkFrameIndex = 0;
                }
                return;
            }
        }

        if (randomizeSprite)
        {
            int randomFrame = Random.Range(0, frameCount);

            while (randomFrame == frameIndex && noRepeat)
            {
                randomFrame = Random.Range(0, frameCount);
            }

            frameIndex = randomFrame;
            SetFrame(randomFrame);
        }
        else
        {
            if (Reversed)
            {
                frameIndex--;
            }
            else
            {
                frameIndex++;
            }
            if ((!Reversed && frameIndex >= frameCount) || (Reversed && frameIndex < 0))
            {
                if (stopAfterSingleIteration)
                {
                    enabled = false;
                    if (Reversed)
                    {
                        frameIndex++;
                    }
                    else
                    {
                        frameIndex--;
                    }
                }
                else
                {
                    if (pingpong)
                    {
                        frameIndex += Reversed ? 1 : -1;
                        Reversed = !Reversed;
                    }
                    else
                    {
                        frameIndex = 0;
                    }
                }
            }

            SetFrame(frameIndex);

            if (frameIndex == soundFrame && !string.IsNullOrEmpty(soundId))
            {
                AudioController.Instance.PlaySound2D(soundId, MixerGroup.None);
            }
        }
    }

	private void Clear()
    {
		this.enabled = false;

        if (sR != null)
        {
		    sR.sprite = null;
        }
        else if (r != null)
        {
            SetTexture(null);
        }
	}

	public void Stop()
    {
		stopOnNextFrame = false;
		this.enabled = false;
        SetFrame(0);
	}

    private void SetFrame(int frameIndex)
    {
        if (sR != null)
        {
            sR.sprite = frames[frameIndex];
        }
        else if (r != null)
        {
            SetTexture(textureFrames[frameIndex]);
        }
    }

    private void SetTexture(Texture2D tex)
    {
        r.material.SetTexture(textureField, tex);
    }
    
    private void UpdateFrameCount()
    {
        frameCount = sR != null ? frames.Count : textureFrames.Count;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        blinkTimer += deltaTime;
        timer += deltaTime;
		if (timer > animSpeed)
        {
            IterateFrame();
		}
        UpdateFrameCount();
	}
}
