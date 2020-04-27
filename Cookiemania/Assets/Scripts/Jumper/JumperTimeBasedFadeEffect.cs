using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JumperTimeBasedFadeEffect : MonoBehaviour
{
    [SerializeField]
    private float updateSpeed = 0.5f;

    [Range(0f, 0.99f)]
    [SerializeField]
    private float minimumAlpha = 0.3f;

    private bool goToMinimum = false;
    private TextMeshProUGUI textRef;
    private float angle = 1f;
    
    private Color baseColor;
    // Start is called before the first frame update
    void Awake()
    {
        textRef = GetComponentInChildren<TextMeshProUGUI>();
        if (textRef == null)
            textRef = GetComponent<TextMeshProUGUI>();
        baseColor = textRef.color;
    }

    // Update is called once per frame
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
        textRef.color = baseColor;
    }
}
