using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


//create other controllers with their inputs exposed only
//attach to the player object and remove other instances of jumper input component already on it
//trying to ensure execution is before player
[DefaultExecutionOrder(-100)]
public abstract class JumperInputComponent : General_Input
{

    #region public functions
    //keys are the nameof the axes the input component uses (Horizontal, Jump etc)
    //contains values that are tuples of lists tuple items: item1 = positive, item2 = negative
    public Dictionary<string, System.Tuple<List<string>, List<string>>> AxesToKeys { get { return axesDictionary; } }
    public float Horizontal { get { return HorizontalInput(); } }
    public float Jump { get { return JumpInput(); } }
    public float Pickup { get { return PickupInput(); } }
    public float Throw { get { return ThrowInput(); } }
    //the names of the positive/negative keys
    public virtual List<string> HorizontalPositiveKeys { get { return leftKeyStrings; } }
    public virtual List<string> HorizontalNegativeKeys { get { return rightKeyStrings; } }
    public virtual List<string> JumpPositiveKeys { get { return jumpKeyStrings; } }
    public virtual List<string> JumpNegativeKeys { get { return jumpNegativeKeyStrings; } }
    public virtual List<string> PickupPositiveKeys { get { return pickupKeyStrings; } }
    public virtual List<string> PickupNegativeKeys { get { return pickupNegativeKeyStrings; } }
    public virtual List<string> ThrowPositiveKeys { get { return throwKeyStrings; } }
    public virtual List<string> ThrowNegativeKeys { get { return throwNegativeKeyStrings; } }

    #endregion

    #region override required
    //need to be overriden
    protected abstract float HorizontalInput();
    protected abstract float JumpInput();
    protected abstract float PickupInput();
    protected abstract float ThrowInput();
    protected abstract void StringifyKeys(string axis, bool isPositive, out List<string> keys);

    #endregion

    #region protected variables


    protected Dictionary<string, System.Tuple<List<string>, List<string>>> axesDictionary = 
        new Dictionary<string, System.Tuple<List<string>, List<string>>>();
    protected float horizontal = 0f;
    protected float jump = 0f;
    protected float pickup = 0f;
    protected float thrown = 0f;
    protected List<string> leftKeyStrings = new List<string>();
    protected List<string> rightKeyStrings = new List<string>();
    protected List<string> jumpKeyStrings = new List<string>();
    protected List<string> jumpNegativeKeyStrings = new List<string>();
    protected List<string> pickupKeyStrings = new List<string>();
    protected List<string> pickupNegativeKeyStrings = new List<string>();
    protected List<string> throwKeyStrings = new List<string>();
    protected List<string> throwNegativeKeyStrings = new List<string>();


    #endregion

    #region protected static

    protected static void CreateStringList<T>(List<T> keys, out List<string> strings)
    {
        strings = new List<string>();
        foreach (var key in keys)
        {
            strings.Add(key.ToString());
        }
    }

    #endregion

    #region virtuals

    protected override void Awake()
    {
        base.Awake();
        StringifyKeys(nameof(Horizontal), true, out rightKeyStrings);
        StringifyKeys(nameof(Horizontal), false, out leftKeyStrings);
        StringifyKeys(nameof(Jump), true, out jumpKeyStrings);
        StringifyKeys(nameof(Jump), false, out jumpNegativeKeyStrings);
        StringifyKeys(nameof(Pickup), true, out pickupKeyStrings);
        StringifyKeys(nameof(Pickup), false, out pickupNegativeKeyStrings);
        StringifyKeys(nameof(Throw), true, out throwKeyStrings);
        StringifyKeys(nameof(Throw), false, out throwNegativeKeyStrings);
        CreateAxesToKeysDictionary(out axesDictionary);
        //to see some of the key stuff
        /*
        foreach (var key in axesDictionary.Keys)
        {
            if (axesDictionary[key].Item1.Count > 0)
            {
                Debug.Log(axesDictionary[key].Item1[0]);
            }
            if (axesDictionary[key].Item2.Count > 0)
            {
                Debug.Log(axesDictionary[key].Item2[0]);
            }
        }
        */
    }

    protected override void SetAxesDelegates()
    {
        axisReturnDelegates = new List<System.Func<float>>(axes.Count);
        for(int i = 0; i < Axes.Count; i++)
        {
            axisReturnDelegates.Add(null);
            var axis = Axes[i];
            switch (axis)
            {
                case nameof(Horizontal):
                    axisReturnDelegates[i] = HorizontalInput;
                    break;
                case nameof(Jump):
                    axisReturnDelegates[i] = JumpInput;
                    break;
                case nameof(Pickup):
                    axisReturnDelegates[i] = PickupInput;
                    break;
                case nameof(Throw):
                    axisReturnDelegates[i] = ThrowInput;
                    break;
                default:
                    Debug.LogWarning("Unsupported axis for jumper input");
                    break;
            }
        }
    }

    protected virtual void CreateAxesToKeysDictionary(out Dictionary<string, System.Tuple<List<string>, List<string>>> dict)
    {
        dict = new Dictionary<string, System.Tuple<List<string>, List<string>>>
        {
            { nameof(Horizontal), new System.Tuple<List<string>, List<string>>(rightKeyStrings, leftKeyStrings) },
            { nameof(Pickup), new System.Tuple<List<string>, List<string>>(pickupKeyStrings, pickupNegativeKeyStrings) },
            { nameof(Throw), new System.Tuple<List<string>, List<string>>(throwKeyStrings, throwNegativeKeyStrings) },
            { nameof(Jump), new System.Tuple<List<string>, List<string>>(jumpKeyStrings, jumpNegativeKeyStrings) }
        };
    }

    #endregion

}
