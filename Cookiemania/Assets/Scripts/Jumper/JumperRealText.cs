using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperRealText : MonoBehaviour
{
    private TMPro.TextMeshPro text = null;
    [SerializeField]
    private float duration = 1.0f;
    [SerializeField]
    private float fadeDuration = 0.25f;
    [SerializeField]
    private float heightAbovePlayer = 1.22f;
    [SerializeField]
    private Color goodColor = Color.yellow;
    [SerializeField]
    private Color badColor = Color.red;
    [SerializeField]
    private Color neutralColor = Color.blue;

    public enum FeedbackType
    {
        Good,
        Bad,
        Neutral
    }

    private float ttl = 0f;
    private Color currentColor = Color.white;
    private Vector3 offset = Vector3.zero;
    private Transform parent = null;

    private void Awake()
    {
        text = GetComponent<TMPro.TextMeshPro>();
        text.text = "";
        transform.parent = null;
        offset = new Vector3(0f, heightAbovePlayer, 0f);
        enabled = false;
        gameObject.SetActive(false);
    }

    /*
     * new idea: gets random velocity and size is based on an intensity argument
     * if no text, uses intensity as its text, parent is what it will reparent to when it deactivates
     * Spawn(Transform parent, int intensity, FeedbackType type, Vector3 position, string text, float dur)
     * 
     * will have a manager that "spawns" it and recycles after
     */

    public void Spawn(Transform dad, int intensity, FeedbackType type, Vector3 pos, string txt, float dur)
    {
        StopAllCoroutines();
        transform.parent = null;
        parent = dad;
        duration = dur > 0f ? dur : duration;
        ttl = duration;
        text.text = txt != "" ? txt : intensity.ToString();
        currentColor = ChangeColor(type);
        text.color = currentColor;
        transform.position = pos + offset;
        enabled = true;
        gameObject.SetActive(true);
    }

    public void Activate(string displayText, FeedbackType feedback, Vector3 position, float dur = 0f)
    {
        StopAllCoroutines();
        duration = dur > 0f ? dur : duration;
        ttl = duration;
        text.text = displayText;
        currentColor = ChangeColor(feedback);
        text.color = currentColor;
        transform.position = position + offset;
        enabled = true;
        gameObject.SetActive(true);
    }

    private Color ChangeColor(FeedbackType feedback)
    {
        switch(feedback)
        {
            case FeedbackType.Bad:
                return badColor;
            case FeedbackType.Good:
                return goodColor;
            case FeedbackType.Neutral:
            default:
                return neutralColor;
        };
    }

    public void Update()
    {
        ttl -= Time.deltaTime;
        if (ttl <= 0f)
        {
            enabled = false;
            StartCoroutine(Fadeout());
        }
    }

    private IEnumerator Fadeout()
    {
        var wait = new WaitForFixedUpdate();
        var updateTime = Time.fixedDeltaTime;
        var elapsed = 0f;
        var alpha = text.color.a;
        while (elapsed < fadeDuration)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 
                Mathf.Lerp(alpha, 0f, elapsed / fadeDuration));
            yield return wait;
            elapsed += updateTime;
        }
        text.text = "";
        transform.parent = parent;
        gameObject.SetActive(false);
    }
}
