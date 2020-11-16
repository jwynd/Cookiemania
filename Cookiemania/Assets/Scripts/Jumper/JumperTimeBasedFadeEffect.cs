using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JumperTimeBasedFadeEffect : MonoBehaviour
{
    [SerializeField]
    private float updateSpeed = 0.5f;

    [Range(0f, 0.99f)]
    [SerializeField]
    private float minimumAlpha = 0.3f;

    [SerializeField]
    private bool useImage = false;

    private bool goToMinimum = false;
    private TextMeshProUGUI textRef;
    private Image imageAlt;
    private float angle = 1f;
    
    private Color baseColor;

    void Awake()
    {
        textRef = GetComponentInChildren<TextMeshProUGUI>();
        if (textRef == null)
            textRef = GetComponent<TextMeshProUGUI>();
        imageAlt = GetComponent<Image>();
        if (useImage)
            baseColor = imageAlt.color;
        else
            baseColor = textRef.color;
    }

    void Update()
    {
        if (angle <= minimumAlpha || angle >= 1f)
        {
            goToMinimum = !goToMinimum;
        }
        if (goToMinimum)
        {
            angle = Mathf.MoveTowards(angle, minimumAlpha, updateSpeed * Time.deltaTime);
        }
        else
        {
            angle = Mathf.MoveTowards(angle, 1f, updateSpeed * Time.deltaTime);
        }
        baseColor.a = angle;
        if (useImage)
            imageAlt.color = baseColor;
        else
            textRef.color = baseColor;
    }
}
