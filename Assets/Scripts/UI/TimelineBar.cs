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
        float rangeDist = Vector2.Distance(leftRangeMarker.position, rightRangeMarker.position);
        float distPerEvent = rangeDist / TimelineController.Instance.NumEventsInActiveRange;
        float eventStartPos = distPerEvent * TimelineController.Instance.ActiveRangeEventProgress;
        float markerX = eventStartPos + TimelineController.Instance.CurrentEvent.EventProgress;
        playerMarker.position = new Vector2(markerX, playerMarker.position.y);
    }
}