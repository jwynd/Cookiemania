using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

using static General_Utilities.ReflectionHelpers;

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
        Initialized = PropertiesNonNull(this, myInfos);
    }

    public void SetCompanyMotto(string name)
    {
        if (!customizable) { return; }

        CompanyMotto = name;
        Initialized = PropertiesNonNull(this, myInfos);

    }

    public void SetCompanyDescription(string name)
    {
        if (!customizable) { return; }

        CompanyDescription = name;
        Initialized = PropertiesNonNull(this, myInfos);

    }

    public void SetIcon(Sprite image)
    {
        if (!customizable) { return; }

        Icon = image;
        Initialized = PropertiesNonNull(this, myInfos);

    }

    public void SetPicture1(Sprite image)
    {
        if (!customizable) { return; }
        Picture1 = image;
        Initialized = PropertiesNonNull(this, myInfos);
    }

    public void SetPicture2(Sprite image)
    {
        if (!customizable) { return; }

        Picture2 = image;
        Initialized = PropertiesNonNull(this, myInfos);

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
        myInfos = GetValidProperties(this);
        customizable = true;
    }

    
}
