using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedMovement : MonoBehaviour
{
    [SerializeField]
    private float startX;

    [SerializeField]
    private float endX;

    private void Update()
    {
        if (!TimelineController.Instance.EndOfTimeline)
        {
            float currentX = Mathf.Lerp(startX, endX, TimelineController.Instance.NormalizedTime);
            transform.position = new Vector2(currentX, transform.position.y);
        }
    }
}