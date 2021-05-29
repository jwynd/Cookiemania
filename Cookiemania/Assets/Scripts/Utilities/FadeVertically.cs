using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
// relies on a shader that fades a sprite vertically in 
// world space
public class FadeVertically : MonoBehaviour
{
    private const string ShaderFadeVar = "FadeHeight";

    [SerializeField]
    private float fadeDuration = 1f;

    private SpriteRenderer sprite;
    private Material material;
    private float fadeTime = 0f;
    private float overridableDuration = 0f;
    private UnityAction runAfterFade;
    private bool effectComplete = true;
    private bool fading;
    private Transform oldParent;
    private Vector3 offset;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        material = sprite.material;
        material.SetFloat(ShaderFadeVar, float.MinValue);
        overridableDuration = fadeDuration;
    }

    private void Start()
    {
        oldParent = transform.parent;
        if (!oldParent) return;
        offset = Vector3.Scale(transform.localPosition, transform.parent.localScale);
        transform.parent = null;
    }

    void LateUpdate()
    {
        if (oldParent) transform.position = oldParent.position + offset;
        if (effectComplete) return;
        fadeTime += Time.deltaTime;
        if (fadeTime >= overridableDuration) FinalizeEffect();
        else LerpEffect();
    }

    private void LerpEffect()
    {
        var fadeTarget = sprite.bounds.size.y * 0.5f + sprite.bounds.center.y;
        var fadeStart = fadeTarget - sprite.bounds.size.y;
        if (!fading)
            material.SetFloat(ShaderFadeVar,
                Mathf.Lerp(fadeTarget, fadeStart, fadeTime / overridableDuration));
        else
            material.SetFloat(ShaderFadeVar,
                Mathf.Lerp(fadeStart, fadeTarget, fadeTime / overridableDuration));
    }

    private void FinalizeEffect()
    {
        if (fading) material.SetFloat(ShaderFadeVar, float.MaxValue);
        else material.SetFloat(ShaderFadeVar, float.MinValue);
        runAfterFade.Invoke();
    }

    public void StartFade(UnityAction runOnComplete, float duration = -1f)
    {
        fading = true;
        effectComplete = false;
        fadeTime = 0f;
        overridableDuration = duration > 0f ? duration : fadeDuration;
        runAfterFade = runOnComplete;
    }

    public void StartAppear(UnityAction runOnComplete, float duration = -1f)
    {
        fading = false;
        effectComplete = false;
        fadeTime = 0f;
        overridableDuration = duration > 0f ? duration : fadeDuration;
        runAfterFade = runOnComplete;
    }
}
