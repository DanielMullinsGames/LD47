using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineController : Singleton<TimelineController>
{
    public TimelineEvent CurrentEvent => events[markerPosition];
    public int NumEventsInActiveRange => (((events.Count / 2) - markerPosition) * 2) + 1;
    public int ActiveRangeEventProgress => markerPosition - rangeStartPosition;

    public float NormalizedTime
    {
        get
        {
            float time = 0f;
            for (int i = 0; i < markerPosition; i++)
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

    private bool ReachedEnd => markerPosition == events.Count;

    [SerializeField]
    private List<TimelineEvent> events = new List<TimelineEvent>();

    private int markerPosition;
    private int rangeStartPosition;

    private void Start()
    {
        markerPosition = rangeStartPosition = Mathf.CeilToInt(events.Count / 2f) - 1;
        StartCoroutine(MainLoop());
    }

    private IEnumerator MainLoop()
    {
        while (!ReachedEnd)
        {
            yield return PlayFromMarker();
        }
    }

    private IEnumerator PlayFromMarker()
    {
        bool survived = true;
        PlayerController.Instance.Anim.Play("idle", 0, 0f);

        while (survived && !ReachedEnd)
        {
            var currentEvent = events[markerPosition];
            yield return currentEvent.PlayEvent();

            survived = currentEvent.Survived;
            if (survived)
            {
                markerPosition++;
            }
        }
    }
}
