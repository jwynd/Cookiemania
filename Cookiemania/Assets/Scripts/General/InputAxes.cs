using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputAxes : MonoBehaviour
{
    public static InputAxes Instance { get; private set; }
    // not sure why this axis got changed ? lol
    public InputAction Fire { get; private set; } = new InputAction(binding: "<Keyboard>/UpArrow");
    public InputAction Jump { get; private set; } = new InputAction(binding: "<Keyboard>/space");
    public InputAction Rotation { get; private set; } = new InputAction();
    public InputAction Horizontal { get; private set; } = new InputAction();
    public InputAction Vertical { get; private set; } = new InputAction();
    public InputAction Action1 { get; private set; } = new InputAction(binding: "<Keyboard>/r");
    public InputAction Action2 { get; private set; } = new InputAction(binding: "<Keyboard>/e");
    public InputAction Action3 { get; private set; } = new InputAction(binding: "<Keyboard>/q");
    public InputAction Action4 { get; private set; } = new InputAction(binding: "<Keyboard>/f");
    public InputAction Escape { get; private set; } = new InputAction(binding: "<Keyboard>/escape");

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Horizontal.AddCompositeBinding("Axis")
            .With("Positive", "<Keyboard>/d")
            .With("Negative", "<Keyboard>/a");
        Vertical.AddCompositeBinding("Axis")
            .With("Positive", "<Keyboard>/w")
            .With("Negative", "<Keyboard>/s");
        Rotation.AddCompositeBinding("Axis")
            .With("Positive", "<Keyboard>/q")
            .With("Negative", "<Keyboard>/e");
        PropertyInfo[] props = typeof(InputAxes).GetProperties();
        foreach (var prop in props)
        {
            // enable all the input actions on the instance
            if (prop.PropertyType == typeof(InputAction))
            {
                ((InputAction)prop.GetValue(this, null)).Enable();
            }
        }
        
    }

    public void RemapButton(InputAction actionToRebind)
    {
        var rebindOperation = actionToRebind.PerformInteractiveRebinding()
                    // To avoid accidental input from mouse motion
                    .WithControlsExcluding("Mouse")
                    .OnMatchWaitForAnother(0.1f)
                    .Start();
    }
}
