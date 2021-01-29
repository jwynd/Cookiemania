using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public abstract class General_Input : MonoBehaviour
{
    //set return delegates in awake
    #region public functions
    //returns the axis name and its current value for each given index
    public System.Tuple<string, float> GetAxis(int i)
    {
        if (i > axes.Count || i > axisReturnDelegates.Count)
        {
            Debug.LogWarning("index OOR for GetGenericAxis");
            return null;
        }
        return new System.Tuple<string, float>(Axes[i], axisReturnDelegates[i]());
    }

    //returns axis' current value for the given named axis (case sensitive)
    public float GetAxis(string axisName)
    {
        return axisReturnDelegates[axisNameToIndex[axisName]]();
    }

    //has the names of all the input axes this listens to
    public List<string> Axes { get { return axes; } }

    public static float GetKeyDown(ICollection<KeyCode> keys)
    {
        foreach (var key in keys)
        {
            if (Input.GetKeyDown(key)) { return 1f; }
        }
        return 0f;
    }

    public static float GetKey(ICollection<KeyCode> keys)
    {
        foreach (var key in keys)
        {
            if (Input.GetKey(key)) { return 1f; }
        }
        return 0f;
    }

    public static float GetKey(ICollection<KeyCode> pos, ICollection<KeyCode> neg)
    {
        float value = 0f;
        foreach (var key in neg)
        {
            if (Input.GetKey(key)) { value--; break; }
        }
        foreach (var key in pos)
        {
            if (Input.GetKey(key)) { value++; break; }
        }
        return value;
    }

    public static float GetKeyDown(ICollection<KeyCode> pos, ICollection<KeyCode> neg)
    {
        float value = 0f;
        foreach (var key in neg)
        {
            if (Input.GetKeyDown(key)) { value--; break; }
        }
        foreach (var key in pos)
        {
            if (Input.GetKeyDown(key)) { value++; break; }
        }
        return value;
    }

    #endregion

    #region protected variables
    protected List<System.Func<float>> axisReturnDelegates = new List<System.Func<float>>();
    protected List<string> axes = new List<string>();
    protected Dictionary<string, int> axisNameToIndex = new Dictionary<string, int>();
    //not to be overriden, put functions in the axisreturndelegates list

    protected void PrintAxes()
    {
        for (int i = 0; i < axes.Count; i++)
        {
            var axis = axes[i];
            Debug.Log(GetAxis(axis));
            Debug.Log(GetAxis(i).Item1);
        }
    }

    #endregion

    #region override required

    //this will work if the axes are properties of type float e.g. ==>
    //public float Horizontal { get; protected set; }
    protected virtual void SetAxes()
    {
        PropertyInfo[] infos = GetType().GetProperties();
        foreach (var info in infos)
        {
            if (info.PropertyType.Equals(typeof(float)))
            {
                axes.Add(info.Name);
            }
        }
    }

    protected virtual void SetAxisNameToIndex()
    {
        for (int i = 0; i < Axes.Count; i++)
        {
            axisNameToIndex.Add(axes[i], i);
        }
    }

    //this needs to be set by the inheriter tbh
    //example is ==> 
    /*
        for(int i = 0; i < Axes.Count; i++)
        {
            var axis = Axes[i];
            axisReturnDelegates.Add(null);
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

    */

    protected abstract void SetAxesDelegates();

    #endregion

    protected virtual void Awake()
    {
        SetAxes();
        SetAxisNameToIndex();
        SetAxesDelegates();
#if UNITY_EDITOR
/*        PrintAxes();
*/
#endif
    }
}
