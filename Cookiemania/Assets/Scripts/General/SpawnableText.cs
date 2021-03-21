using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableText : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshPro text;

    public enum SpawnState { Alive, Dead, FadingOut }
    public Vector3 Velocity { get; private set; }
    public Vector3 Acceleration { get; private set; }
    public SpawnState State { get; private set; } = SpawnState.Dead;

    // time til it starts dying (fading out)
    private float ttf;
    // time til it completes lifecycle (alive + fadingout)
    private float ttl;
    private float fadeDuration;
    private float age;
    private Transform spawner;
    private Vector3 originalScale = Vector3.one;
    private float alpha = 1f;

    private void Awake()
    {
        if (!text)
            text = GetComponent<TMPro.TextMeshPro>();
        enabled = false;
        originalScale = transform.localScale;
    }

    // only accelerates while not fading btw
    public void Spawn(Vector2 pos, Vector2 velocity, Vector2 acceleration,
        float sizeScalar, string textToDisplay, Color color,
        // the unparent argument just means the object should unparent until it recycles itself
        float timeUntilFade, float timeToFadeOut, Transform parent = null, bool unparent = true)
    {
        State = SpawnState.Alive;
        if (parent)
            spawner = parent;
        Velocity = new Vector3(velocity.x, velocity.y, 0);
        Acceleration = new Vector3(acceleration.x, acceleration.y, 0);
        fadeDuration = timeToFadeOut;
        ttl = timeToFadeOut + timeUntilFade;
        ttf = timeUntilFade;
        if (unparent)
            transform.SetParent(null);
        transform.position = pos;
        transform.localScale = originalScale * sizeScalar;
        transform.rotation = Quaternion.identity;
        text.text = textToDisplay;
        text.color = color;
        alpha = color.a;
        gameObject.SetActive(true);
        age = 0f;
    }

    public void Tick(float delta)
    {
        age += delta;
        // killing time
        if (age > ttl)
        {
            State = SpawnState.Dead;
            Recycle();
            return;
        }
        Move(delta);
        // extra fading actions
        if (age > ttf)
        {
            State = SpawnState.FadingOut;
            text.color = Fade(age - ttf, fadeDuration, alpha, text.color);
        }
    }

    public void Recycle(Transform parent = null)
    {
        if (parent)
            spawner = parent;
        gameObject.SetActive(false);
        State = SpawnState.Dead;
        transform.SetParent(spawner, false);
    }

    private Color Fade(float fadeTimeElapsed, float totalFadeTime, 
        float originalAlpha, Color color)
    {
        var deduction = fadeTimeElapsed / totalFadeTime;
        return new Color(color.r, color.g, color.b, 
            originalAlpha - (deduction * originalAlpha));
    }

    private void Move(float delta)
    {
        Velocity += Acceleration * delta;
        transform.position += Velocity * delta;
    }

}
