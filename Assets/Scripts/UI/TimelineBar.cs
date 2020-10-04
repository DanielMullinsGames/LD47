using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

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

    [SerializeField]
    private SpriteRenderer activeAreaBar;

    [SerializeField]
    private AnimationCurve tweenCurve;

    [SerializeField]
    private Animator frameAnim;

    private bool tweening = false;

    private void Start()
    {
        ShowRange(TimelineController.Instance.RangeStartIndex, TimelineController.Instance.RangeEndIndex);
    }

    public IEnumerator ExpandEntireTimeline()
    {
        frameAnim.Play("expand", 0, 0f);
        yield return new WaitForSeconds(3f);
    }

    public IEnumerator ShowExpandRange(int rangeStartIndex, int rangeEndIndex)
    {
        const float duration = 2f;

        ShowRange(rangeStartIndex, rangeEndIndex, immediate: false, duration: duration);
        yield return new WaitForSeconds(duration);
    }

    private void ShowRange(int rangeStartIndex, int rangeEndIndex, bool immediate = true, float duration = 1f)
    {
        Vector2 leftRangePos = new Vector2(GetTimelineX(rangeStartIndex), leftRangeMarker.position.y);
        Vector2 rightRangePos = new Vector2(GetTimelineX(rangeEndIndex), rightRangeMarker.position.y);

        float barWidth = rightRangePos.x - leftRangePos.x;

        if (immediate)
        {
            leftRangeMarker.position = leftRangePos;
            rightRangeMarker.position = rightRangePos;

            SetBarWidth(barWidth);
        }
        else
        {
            StartCoroutine(TweenUI(leftRangePos, rightRangePos, barWidth, duration));
        }
    }

    private float GetTimelineX(int timelineIndex)
    {
        float timelineProgress = timelineIndex / (float)TimelineController.Instance.NumEventsInTimeline;
        return Mathf.Lerp(leftBarEdge.position.x, rightBarEdge.position.x, timelineProgress);
    }

    private void Update()
    {
        if (!TimelineController.Instance.EndOfTimeline && !PlayerController.Instance.Dead && !tweening)
        {
            int numEventsInRange = TimelineController.Instance.NumEventsInActiveRange;
            float timelineProgress = TimelineController.Instance.ActiveRangeEventProgress / (float)numEventsInRange;
            float eventProgress = TimelineController.Instance.CurrentEvent.EventProgress * (1f / numEventsInRange);
            float markerX = Mathf.Lerp(leftRangeMarker.position.x, rightRangeMarker.position.x, timelineProgress + eventProgress);
            playerMarker.position = new Vector2(markerX, playerMarker.position.y);
        }
    }

    private IEnumerator TweenUI(Vector2 leftMarkerPos, Vector2 rightMarkerPos, float barWidth, float duration)
    {
        tweening = true;
        float timer = 0f;

        Vector2 startLeftPos = leftRangeMarker.position;
        Vector2 startRightPos = rightRangeMarker.position;
        float startBarWidth = activeAreaBar.size.x;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = tweenCurve.Evaluate(timer / duration);

            leftRangeMarker.position = Vector2.Lerp(startLeftPos, leftMarkerPos, progress);
            rightRangeMarker.position = Vector2.Lerp(startRightPos, rightMarkerPos, progress);
            SetBarWidth(Mathf.Lerp(startBarWidth, barWidth, progress));

            yield return new WaitForEndOfFrame();
        }

        leftRangeMarker.position = leftMarkerPos;
        rightRangeMarker.position = rightMarkerPos;
        SetBarWidth(barWidth);
        tweening = false;
    }

    private void SetBarWidth(float barWidth)
    {
        var size = activeAreaBar.size;
        size.x = barWidth;
        activeAreaBar.size = size;
    }
}