using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JumperTimeBasedFadeEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum EUseCase : uint
    {
        Sprite,
        Image,
        Text,
    }

    private const float MAX_ALPHA = 1f;
    [SerializeField]
    private float updateSpeed = 0.5f;

    [Range(0f, 0.99f)]
    [SerializeField]
    private float minimumAlpha = 0.3f;

    [SerializeField]
    private EUseCase useCase = EUseCase.Text;

    [SerializeField]
    private bool disableOnHover = false;

    private bool goToMinimum = false;
    private bool effectDisabled = false;
    private TextMeshProUGUI textRef;
    private Image imageAlt;
    private SpriteRenderer spriteAlt;
    private float angle = MAX_ALPHA;
    
    private Color baseColor;

    void Awake()
    {
        // prefer text in child, image in parent
        textRef = GetComponentInChildren<TextMeshProUGUI>();
        imageAlt = GetComponent<Image>();
        spriteAlt = GetComponent<SpriteRenderer>();
        if (textRef == null)
            textRef = GetComponent<TextMeshProUGUI>();

        if (imageAlt == null)
            imageAlt = GetComponentInChildren<Image>();
        switch (useCase) 
        {
            case EUseCase.Image:
                baseColor = imageAlt.color;
                break;
            case EUseCase.Text:
                baseColor = textRef.color;
                break;
            case EUseCase.Sprite:
                baseColor = spriteAlt.color;
                break;
        }
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
        switch(useCase)
        {
            case EUseCase.Text:
                textRef.color = baseColor;
                break;
            case EUseCase.Image:
                imageAlt.color = baseColor;
                break;
            case EUseCase.Sprite:
                spriteAlt.color = baseColor;
                break;
        }
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
