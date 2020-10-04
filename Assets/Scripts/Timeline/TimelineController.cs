using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineController : Singleton<TimelineController>
{
    public float NormalizedTime
    {
        get
        {
            float time = 0f;
            for (int i = 0; i < currentPositionIndex; i++)
            {
                time += events[i].Length;
            }
            time += CurrentEvent.Length * CurrentEvent.EventProgress;
            return time / TimelineTotalLength;
        }
    }

    private float TimelineTotalLength
    {
        get
        {
            float sum = 0f;
            events.ForEach(x => sum += x.Length);
            return sum;
        }
    }

    public bool EndOfTimeline => CurrentEvent == null;
    public TimelineEvent CurrentEvent => currentPositionIndex < events.Count ? events[currentPositionIndex] : null;
    public int NumEventsInTimeline => events.Count;
    public int NumEventsInActiveRange => (((events.Count / 2) - RangeStartIndex) * 2) + 1;
    public int ActiveRangeEventProgress => currentPositionIndex - RangeStartIndex;
    public int RangeStartIndex { get; private set; }
    public int RangeEndIndex => RangeCenterIndex + (RangeCenterIndex - RangeStartIndex + 1);
    private int RangeCenterIndex => Mathf.CeilToInt(events.Count / 2f) - 1;

    [SerializeField]
    private List<TimelineEvent> events = new List<TimelineEvent>();

    private int currentPositionIndex;

#if UNITY_EDITOR
    [Header("DEBUG")]
    public int debugStartRangeIndex;

    public int debugStartMarker;
#endif

    private void Start()
    {
        currentPositionIndex = RangeStartIndex = RangeCenterIndex;

#if UNITY_EDITOR
        RangeStartIndex = debugStartRangeIndex;
        currentPositionIndex = debugStartMarker;
#endif

        SkipToStartOfEvent(currentPositionIndex);
        StartCoroutine(MainLoop());
    }

    private IEnumerator MainLoop()
    {
        while (!EndOfTimeline)
        {
            yield return PlayFromMarker();
        }
    }

    private IEnumerator PlayFromMarker()
    {
        CameraEffects.Instance.ShowRewind();
        PlayerController.Instance.Reset();
        SkipToStartOfEvent(RangeStartIndex);

        bool survived = true;
        currentPositionIndex = RangeStartIndex;

#if UNITY_EDITOR
        if (debugStartMarker > 0)
        {
            currentPositionIndex = debugStartMarker;
            SkipToStartOfEvent(debugStartMarker);
        }
#endif

        while (survived && !EndOfTimeline)
        {
            float moveTime = CameraController.Instance.MoveToPoint(CurrentEvent.transform.position.x, immediate: currentPositionIndex == RangeStartIndex);
            yield return new WaitForSeconds(moveTime);

            yield return CurrentEvent.PlayEvent();

            survived = CurrentEvent.Survived;
            if (survived)
            {
                currentPositionIndex++;

                if (EndOfTimeline)
                {
                    // END
                    AnimationPauser.Instance.SetPaused(true);
                    break;
                }
                else if (currentPositionIndex >= RangeEndIndex)
                {
                    yield return ExpandActiveRange();
                    break;
                }
            }
        }
    }

    private IEnumerator ExpandActiveRange()
    {
        AnimationPauser.Instance.SetPaused(true);
        yield return new WaitForSeconds(0.5f);

        RangeStartIndex = Mathf.Max(0, RangeStartIndex - 1);
        yield return TimelineBar.Instance.ShowExpandRange(RangeStartIndex, RangeEndIndex);
        yield return new WaitForSeconds(0.25f);

        yield return StepBackToStartMarker();
        yield return new WaitForSeconds(0.25f);

        AnimationPauser.Instance.SetPaused(false);
    }

    private IEnumerator StepBackToStartMarker()
    {
        while (currentPositionIndex > RangeStartIndex)
        {
            currentPositionIndex--;
            CameraEffects.Instance.ShowRewind();
            SkipToStartOfEvent(currentPositionIndex);
            yield return new WaitForSeconds(0.4f);
        }
    }

    private void SkipToStartOfEvent(int index)
    {
        AudioController.Instance.BaseLoopSource.time = NormalizedTime * 40f;
        AudioController.Instance.PlaySound2D("reverse", volume: 0.7f, pitch: new AudioParams.Pitch(AudioParams.Pitch.Variation.Small));

        PlayerController.Instance.Reset();
        CameraController.Instance.MoveToPoint(events[index].transform.position.x, immediate: true);
        for (int i = 0; i < events.Count; i++)
        {
            if (i >= index)
            {
                events[i].ResetEventToStart();
            }
            else
            {
                events[i].ResetEventToEnd();
            }
        }
    }
}
