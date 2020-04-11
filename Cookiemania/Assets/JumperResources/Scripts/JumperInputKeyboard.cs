using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperInputKeyboard : JumperInputComponent
{
    #region serialized variables

    [SerializeField]
    protected List<KeyCode> leftKeys = new List<KeyCode>();
    [SerializeField]
    protected List<KeyCode> rightKeys = new List<KeyCode>();
    [SerializeField]
    protected List<KeyCode> jumpKeys = new List<KeyCode>();
    [SerializeField]
    protected List<KeyCode> pickupKeys = new List<KeyCode>();
    [SerializeField]
    protected List<KeyCode> throwKeys = new List<KeyCode>();

    #endregion

    #region input overrides

    protected virtual float KeyDown(List<KeyCode> keys)
    {
        foreach (var key in keys)
        {
            if (Input.GetKeyDown(key)) { return 1f; }
        }
        return 0f;
    }

    protected virtual float Key(List<KeyCode> keys)
    {
        foreach (var key in keys)
        {
            if (Input.GetKey(key)) { return 1f; }
        }
        return 0f;
    }

    protected override float HorizontalInput()
    {
        float value = 0f;
        foreach(var key in leftKeys)
        {
            if (Input.GetKey(key)) { value--; break; }
        }
        foreach(var key in rightKeys)
        {
            if (Input.GetKey(key)) { value++; break; }
        }
        return value;
    }

    protected override float JumpInput()
    {
        return KeyDown(jumpKeys);
    }

    protected override float ThrowInput()
    {
        return KeyDown(throwKeys);
    }

    protected override float PickupInput()
    {
        return Key(pickupKeys);
    }

    protected override void StringifyKeys(string axis, bool isPositive, out List<string> keys)
    {
        keys = new List<string>();
        switch (axis)
        {
            case nameof(Horizontal):
                if (isPositive) { CreateStringList(rightKeys, out keys); }
                else { CreateStringList(leftKeys, out keys); }
                break;
            case nameof(Jump):
                if (isPositive) { CreateStringList(jumpKeys, out keys); }
                break;
            case nameof(Pickup):
                if (isPositive) { CreateStringList(pickupKeys, out keys); }
                break;
            case nameof(Throw):
                if (isPositive) { CreateStringList(throwKeys, out keys); }
                break;
        }
    }

    #endregion

    #region startup functions

    #endregion

}
