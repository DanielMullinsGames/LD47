using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineBar : Singleton<TimelineBar>
{
    [SerializeField]
    private Transform playerMarker;

    [SerializeField]
    private Transform leftRangeMarker;

    [SerializeField]
    private Transform rightRangeMarker;

    [SerializeField]
    private Transform leftBarEdge;

    [SerializeField]
    private Transform rightBarEdge;

    private void Start()
    {
        ShowRange(TimelineController.Instance.RangeStartIndex, TimelineController.Instance.RangeEndIndex);
    }

    public IEnumerator ShowExpandRange(int rangeStartIndex, int rangeEndIndex)
    {
        ShowRange(rangeStartIndex, rangeEndIndex);
        yield break;
    }

    private void ShowRange(int rangeStartIndex, int rangeEndIndex, bool immediate = true)
    {
        Vector2 leftRangePos = new Vector2(GetTimelineX(rangeStartIndex), leftRangeMarker.position.y);
        Vector2 rightRangePos = new Vector2(GetTimelineX(rangeEndIndex), rightRangeMarker.position.y);

        if (immediate)
        {
            leftRangeMarker.position = leftRangePos;
            rightRangeMarker.position = rightRangePos;
        }
    }

    private float GetTimelineX(int timelineIndex)
    {
        float timelineProgress = timelineIndex / (float)TimelineController.Instance.NumEventsInTimeline;
        return Mathf.Lerp(leftBarEdge.position.x, rightBarEdge.position.x, timelineProgress);
    }

    private void Update()
    {
        if (TimelineController.Instance.CurrentEvent.Survived)
        {
            int numEventsInRange = TimelineController.Instance.NumEventsInActiveRange;
            float timelineProgress = TimelineController.Instance.ActiveRangeEventProgress / (float)numEventsInRange;
            float eventProgress = TimelineController.Instance.CurrentEvent.EventProgress * (1f / numEventsInRange);
            float markerX = Mathf.Lerp(leftRangeMarker.position.x, rightRangeMarker.position.x, timelineProgress + eventProgress);
            playerMarker.position = new Vector2(markerX, playerMarker.position.y);
        }
    }
}