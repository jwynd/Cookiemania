using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AbilityCooldown : MonoBehaviour
{
    [Serializable]
    public enum Abilities
    {
        Magnet,
        JumperShield
    }

    [SerializeField]
    private Abilities abilityType;

    private Image image;
    private float fadeTime = 0f;
    private float fadeDuration;
    private UnityAction runAfterFade;
    private float fadeTarget;
    private float fadeStart;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        bool enabler;
        switch(abilityType)
        {
            case Abilities.JumperShield:
                enabler = PlayerData.Player.JShield > 0;
                break;
            case Abilities.Magnet:
                enabler = PlayerData.Player.JMagnet > 0;
                break;
            default:
                Debug.LogError("unimplemented ability type");
                return;
        }
        gameObject.SetActive(enabler);
        enabled = false;
    }

    void Update()
    {
        fadeTime += Time.deltaTime;
        if (fadeTime >= fadeDuration) FinalizeEffect();
        else LerpEffect();
    }

    private void LerpEffect()
    {
        image.fillAmount =
                Mathf.Lerp(fadeStart, fadeTarget, fadeTime / fadeDuration);
    }

    private void FinalizeEffect()
    {
        image.fillAmount = fadeTarget;
        runAfterFade.Invoke();
        enabled = false;
    }

    public void StartFade(UnityAction runOnComplete, float duration)
    {
        enabled = true;
        fadeStart = 1f;
        fadeTarget = 0f;
        fadeTime = 0f;
        fadeDuration = duration;
        runAfterFade = runOnComplete;
    }

    public void StartAppear(UnityAction runOnComplete, float duration)
    {
        enabled = true;
        fadeTarget = 1f;
        fadeStart = 0f;
        fadeTime = 0f;
        fadeDuration = duration;
        runAfterFade = runOnComplete;
    }
}
