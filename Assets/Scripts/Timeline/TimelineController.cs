using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineController : MonoBehaviour
{
    private bool ReachedEnd => markerPosition == events.Count;

    [SerializeField]
    private List<TimelineEvent> events = new List<TimelineEvent>();

    private int markerPosition;

    private void Start()
    {
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
