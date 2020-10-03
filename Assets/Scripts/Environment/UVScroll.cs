using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVScroll : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f;

    float currentScroll = 0f;

    private void Update()
    {
        currentScroll += speed * Time.deltaTime;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(currentScroll, 0);
    }
}
