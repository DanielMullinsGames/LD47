using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineController : Singleton<TimelineController>
{
    public TimelineEvent CurrentEvent => events[markerIndex];
    public int NumEventsInTimeline => events.Count;
    public int NumEventsInActiveRange => (((events.Count / 2) - markerIndex) * 2) + 1;
    public int ActiveRangeEventProgress => markerIndex - RangeStartIndex;

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

    public int RangeStartIndex { get; private set; }
    public int RangeEndIndex => RangeCenterIndex + (RangeCenterIndex - RangeStartIndex + 1);
    private bool EndOfTimeline => markerIndex == events.Count;
    private int RangeCenterIndex => Mathf.CeilToInt(events.Count / 2f) - 1;

    [SerializeField]
    private List<TimelineEvent> events = new List<TimelineEvent>();

    private int markerIndex;

    private void Start()
    {
        markerIndex = RangeStartIndex = RangeCenterIndex;
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
        RangeStartIndex = Mathf.Max(0, RangeStartIndex - 1);
        yield return TimelineBar.Instance.ShowExpandRange(RangeStartIndex, RangeEndIndex);
    }
}
