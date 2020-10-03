using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineBar : MonoBehaviour
{
    [SerializeField]
    private Transform playerMarker;

    [SerializeField]
    private Transform leftRangeMarker;

    [SerializeField]
    private Transform rightRangeMarker;

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