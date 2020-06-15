using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using static General_Utilities.ReflectionHelpers;
using System;

public class CustomizationUI : MonoBehaviour
{
    [SerializeField]
    protected Image icon = null;
    [SerializeField]
    protected Image picture1 = null;
    [SerializeField]
    protected Image picture2 = null;
    [SerializeField]
    protected List<GameObject> toDisableWhenActive = new List<GameObject>();
    [SerializeField]
    protected GameObject errPanel = null;
    protected Canvas errorCanvas = null;
    protected System.Action runOnEnd = null;

    public static CustomizationUI Instance { get; protected set; }
    public string CompanyName { get; protected set; } = null;
    public string CompanyMotto { get; protected set; } = null;
    public string CompanyDescription { get; protected set; } = null;
    public Sprite Icon { get { return icon.sprite; } }
    public Sprite Picture1 { get { return picture1.sprite; } }
    public Sprite Picture2 { get { return picture2.sprite; } }

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

    public void SetDisableCanvases(List<GameObject> list)
    {
        toDisableWhenActive = list;
    }

    //in order in the class declaration---> name, motto, description
    public List<string> GetTexts()
    {
        var intermediate = GetValidFields<string>(this);
        var toReturn = new List<string>();
        foreach (var i in intermediate)
        {
            //ignoring the tag/name, the default strings
            if (i.Item1 == "tag" || i.Item1 == "name")
                continue;
            Debug.Log(i.Item1 + "   " + i.Item2);
            toReturn.Add(i.Item2);
        }
        return toReturn;
    }

    //also in order of class declaration---> icon, pic1, pic2
    public List<Sprite> GetSprites()
    {
        var intermediate = GetValidFields<Sprite>(this);
        var toReturn = new List<Sprite>();
        foreach (var i in intermediate)
        {
            
            toReturn.Add(i.Item2);
        }
        return toReturn;
    }


    public void CustomizationStart(System.Action runOnComplete = null)
    {
        StateChangeHelper(true);
        if (runOnComplete != null)
            runOnEnd = runOnComplete;
    }

    public void CustomizationFinished()
    {
        if (!PropertiesNonNull(this, myInfos))
        {
            StartCoroutine(Error());
        }
        else if (CompanyName.Length < 2 || CompanyMotto.Length < 2 || CompanyDescription.Length < 2)
        {
            StartCoroutine(Error());
        }
        else
        {
            StopAllCoroutines();
            StateChangeHelper(false);
            if (runOnEnd != null)
                runOnEnd.Invoke();
        }
        //else return an error that not everything has been set
    }

    private void StateChangeHelper(bool enabled)
    {
        customizable = enabled;
        GetComponent<Canvas>().enabled = enabled;
        foreach (var item in toDisableWhenActive)
        {
            try
            {
                item.GetComponent<Canvas>().enabled = !enabled;
            }
            catch
            {
                Debug.LogError("disable target needs to be have a canvas");
            }
        }
    }

    IEnumerator Error()
    {
        errorCanvas.enabled = true;
        float fluc = 0.25f;
        float flucMid = 0.5f;
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSecondsRealtime(0.15f);
            var mats = errPanel.GetComponentInChildren<Image>();
            fluc *= -1f;
            mats.color = new Color(mats.color.r, mats.color.r, mats.color.r, fluc + flucMid);
        }
        errorCanvas.enabled = false;
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
        errorCanvas = errPanel.GetComponent<Canvas>();
        errorCanvas.enabled = false;
        myInfos = GetValidProperties(this);
    }


}
