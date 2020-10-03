using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineController : Singleton<TimelineController>
{
    public TimelineEvent CurrentEvent => events[markerIndex];
    public int NumEventsInActiveRange => (((events.Count / 2) - markerIndex) * 2) + 1;
    public int ActiveRangeEventProgress => markerIndex - rangeStartIndex;

    public float NormalizedTime
    {
        get
        {
            float time = 0f;
            for (int i = 0; i < markerIndex; i++)
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

    private bool EndOfTimeline => markerIndex == events.Count;
    private int RangeCenterIndex => Mathf.CeilToInt(events.Count / 2f) - 1;
    private int RangeEndIndex => RangeCenterIndex + (RangeCenterIndex - markerIndex);

    [SerializeField]
    private List<TimelineEvent> events = new List<TimelineEvent>();

    private int markerIndex;
    private int rangeStartIndex;

    private void Start()
    {
        markerIndex = rangeStartIndex = RangeCenterIndex;
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
        bool survived = true;
        PlayerController.Instance.Reset();

        while (survived && !EndOfTimeline)
        {
            var currentEvent = events[markerIndex];
            yield return currentEvent.PlayEvent();

            survived = currentEvent.Survived;
            if (survived)
            {
                markerIndex++;

                if (markerIndex > RangeEndIndex)
                {
                    yield return ExpandActiveRange();
                    break;
                }
            }
        }
    }

    private IEnumerator ExpandActiveRange()
    {
        rangeStartIndex = Mathf.Max(0, rangeStartIndex - 1);
        yield return TimelineBar.Instance.ShowExpandRange(rangeStartIndex, RangeEndIndex);
    }
}
