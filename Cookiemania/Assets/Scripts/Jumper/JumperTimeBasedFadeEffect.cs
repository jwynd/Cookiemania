using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JumperTimeBasedFadeEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const float MAX_ALPHA = 1f;
    [SerializeField]
    private float updateSpeed = 0.5f;

    [Range(0f, 0.99f)]
    [SerializeField]
    private float minimumAlpha = 0.3f;

    [SerializeField]
    [Tooltip("uses TMP text instead if false")]
    private bool useImage = false;

    [SerializeField]
    private bool disableOnHover = false;

    private bool goToMinimum = false;
    private bool effectDisabled = false;
    private TextMeshProUGUI textRef;
    private Image imageAlt;
    private float angle = MAX_ALPHA;
    
    private Color baseColor;

    void Awake()
    {
        // prefer text in child, image in parent
        textRef = GetComponentInChildren<TextMeshProUGUI>();
        if (textRef == null)
            textRef = GetComponent<TextMeshProUGUI>();
        imageAlt = GetComponent<Image>();
        if (imageAlt == null)
            imageAlt = GetComponentInChildren<Image>();
        if (useImage)
            baseColor = imageAlt.color;
        else
            baseColor = textRef.color;
    }

    void Update()
    {
        if (effectDisabled)
            return;
        UpdateAngle(ref angle, ref goToMinimum, 
            minimumAlpha, updateSpeed, Time.deltaTime);
        SetAlpha(angle);
    }

    private static void UpdateAngle(ref float currentAngle, ref bool towardsMinimum, 
        float minAlpha, float speed, float delta)
    {
        if (currentAngle <= minAlpha || currentAngle >= MAX_ALPHA)
        {
            towardsMinimum = !towardsMinimum;
        }
        currentAngle = Mathf.MoveTowards(
            currentAngle, 
            towardsMinimum ? minAlpha : MAX_ALPHA, 
            speed * delta);
    }

    private void SetAlpha(float alpha)
    {
        baseColor.a = alpha;
        if (useImage)
            imageAlt.color = baseColor;
        else
            textRef.color = baseColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!disableOnHover)
            return;
        effectDisabled = true;
        angle = MAX_ALPHA;
        SetAlpha(angle);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!disableOnHover)
            return;
        effectDisabled = false;
    }
}
