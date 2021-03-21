using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextParticleSystem : MonoBehaviour
{
    [Header("The base text object")]
    [SerializeField]
    private SpawnableText textObj = null;

    [Header("Pool instantiation fields")]
    [SerializeField]
    private int poolSize = 20;
    [SerializeField]
    private float positionOffset = 0.15f;
    [SerializeField]
    private Vector2 velXRange = new Vector2(-2.2f, 2.2f);
    [SerializeField]
    private Vector2 velYRange = new Vector2(-2.2f, 2.2f);
    [SerializeField]
    private Vector2 accelXRange = new Vector2(-2.2f, 2.2f);
    [SerializeField]
    private Vector2 accelYRange = new Vector2(-2.2f, 2.2f);

    [Header("Default arguments that 'Spawn' will use and can be changed by requester")]
    [SerializeField]
    private float timeTilFade = 0.75f;
    [SerializeField]
    private float fadeDuration = 0.15f;

    private List<SpawnableText> texts = new List<SpawnableText>();
    private int nextIndex = 0;

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            // create
            texts.Add(Instantiate(textObj));

            // recycle immediately, use optional arg to tell it to parent to me
            texts[i].Recycle(transform);
        }
    }

    void FixedUpdate()
    {
        // run spawnable ticks
        for (int i = 0; i < texts.Count; i++)
        {
            if (texts[i].State != SpawnableText.SpawnState.Dead) 
                texts[i].Tick(Time.fixedDeltaTime);
        }
    }

    // the function chain --> emit easy uses the position of the text particle system, 
    // emit default allows position argument, emit allows all possible arguments

    public void EmitEasy(float sizeScalar, string displayText, Color color)
    {
        EmitDefault(transform.position, sizeScalar, displayText, color);
    }

    public void EmitDefault(Vector2 pos, float sizeScalar, string displayText, Color color)
    {
        Vector2 velocity = new Vector2(
            Random.Range(velXRange.x, velXRange.y),
            Random.Range(velYRange.x, velYRange.y));
        Vector2 accel = new Vector2(
            Random.Range(accelXRange.x, accelXRange.y),
            Random.Range(accelYRange.x, accelYRange.y));
        Emit(pos, sizeScalar, displayText, color, 
            velocity, accel, timeTilFade, fadeDuration);
    }

    public void Emit(Vector2 pos, float sizeScalar, string displayText, 
        Color color, Vector2 vel, Vector2 accel, float timeTilFade, float fadeDuration, bool useSpacing = true)
    {
        if (useSpacing)
        {
            var spacing = new Vector2(
                Random.Range(-positionOffset, positionOffset), 
                Random.Range(-positionOffset, positionOffset));
            pos += spacing;
        }
        texts[nextIndex].Spawn(pos, vel, accel, sizeScalar, 
            displayText, color, timeTilFade, fadeDuration);
        nextIndex = (nextIndex + 1) % texts.Count;
    }
}
