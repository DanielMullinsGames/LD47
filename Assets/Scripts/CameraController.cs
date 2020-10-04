using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class CameraController : Singleton<CameraController>
{
	public float MoveToPoint(float pointX, bool immediate = false)
    {
        Vector3 pos = new Vector3(pointX, transform.parent.position.y, transform.parent.position.z);

        if (immediate)
        {
            transform.parent.position = pos;
            return 0f;
        }
        else
        {
            float dist = Mathf.Abs(transform.parent.position.x - pointX);
            float duration = dist * 0.05f;
            PlayerController.Instance.StartMove();
            Tween.Position(transform.parent, pos, duration, 0f, Tween.EaseInOut, completeCallback: () => PlayerController.Instance.EndMove());
            return duration;
        }
    }
}