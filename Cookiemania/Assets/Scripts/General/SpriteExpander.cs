using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteExpander : MonoBehaviour
{
    private float Duration = 0.3f;

    private float timeElapsed = 0f;
    private float currentScale = 0.2f;
    private Vector3 originalScale = Vector3.one;
    private Vector3 goalScale = Vector3.one;
    private Vector3 originalPos = Vector3.zero;
    private Vector3 goalPos = Vector3.one;

    void Awake()
    {
        transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        enabled = false;
    }

    public void RequestResize(float duration, float startScale, float endScale)
    {
        Duration = duration;
        timeElapsed = 0f;
        originalScale = new Vector3(startScale, startScale, startScale);
        goalScale = new Vector3(endScale, endScale, endScale);
        goalPos = new Vector3(0f, endScale, 0f);
        enabled = true;
        gameObject.SetActive(true);
    }

    void Expand()
    {
        if (timeElapsed >= Duration)
        {
            transform.localScale = goalScale;
            transform.localPosition = goalPos;
            enabled = false;
            return;
        }
        transform.localScale = Vector3.Lerp(originalScale, goalScale, timeElapsed / Duration);
        transform.localPosition = Vector3.Lerp(originalPos, goalPos, timeElapsed / Duration);
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        Expand();
    }

}
