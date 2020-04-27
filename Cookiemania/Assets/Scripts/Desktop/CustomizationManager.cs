using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

using static UnityEngine.Debug;

public class CustomizationManager : MonoBehaviour
{
    public static CustomizationManager Instance { get; protected set; }
    public string CompanyName { get; protected set; } = null;
    public string CompanyMotto { get; protected set; } = null;
    public string CompanyDescription { get; protected set; } = null;
    public Sprite Icon { get; protected set; } = null;
    public Sprite Picture1 { get; protected set; } = null;
    public Sprite Picture2 { get; protected set; } = null;

    public bool Initialized { get; protected set; } = false;

    public void SetCompanyName(string name)
    {
        if (!customizable) { return; }

        CompanyName = name;
        Initialized = IsFullyInitialized(this, myInfos);
    }

    public void SetCompanyMotto(string name)
    {
        if (!customizable) { return; }

        CompanyMotto = name;
        Initialized = IsFullyInitialized(this, myInfos);

    }

    public void SetCompanyDescription(string name)
    {
        if (!customizable) { return; }

        CompanyDescription = name;
        Initialized = IsFullyInitialized(this, myInfos);

    }

    public void SetIcon(Sprite image)
    {
        if (!customizable) { return; }

        Icon = image;
        Initialized = IsFullyInitialized(this, myInfos);

    }

    public void SetPicture1(Sprite image)
    {
        if (!customizable) { return; }
        Picture1 = image;
        Initialized = IsFullyInitialized(this, myInfos);
    }

    public void SetPicture2(Sprite image)
    {
        if (!customizable) { return; }

        Picture2 = image;
        Initialized = IsFullyInitialized(this, myInfos);

    }

    public void CustomizationStart()
    {
        customizable = true;
    }

    public void CustomizationFinished()
    {
        customizable = false;
    }

    protected List<PropertyInfo> myInfos = new List<PropertyInfo>();
    private bool customizable = false;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
        GetInfos();
        customizable = true;
    }

    public static bool IsFullyInitialized(object propertyHolder, IEnumerable<PropertyInfo> list)
    {
        foreach (var info in list)
        {
            object b = null;
            try
            {
                b = info.GetValue(propertyHolder);
            }
            catch
            {
                LogWarning("failed to get value, remove deprecated properties from list");
            }
            if (b == null)
            {
                return false;
            }
        }
        return true;
    }

    private void GetInfos()
    {
        var temp = GetType().GetProperties();
        foreach (var i in temp)
        {
            object b = null;
            bool addThis = true;
            try
            {
                b = i.GetValue(this);
            }
            catch
            {
                Log("get value not supported for component type " + i.PropertyType);
                addThis = false;
            }
            if (addThis && b == null)
            {
                myInfos.Add(i);
                Log(i);
            }
        }
    }
}
