using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using static General_Utilities.ReflectionHelpers;

public class CustomizationManager : MonoBehaviour
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

    public static CustomizationManager Instance { get; protected set; }
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



    public void CustomizationStart()
    {
        customizable = true;
        gameObject.SetActive(true);
        foreach (var item in toDisableWhenActive)
        {
            item.SetActive(false);
        }
    }

    public void CustomizationFinished()
    {
        if (PropertiesNonNull(this, myInfos))
        {
            customizable = false;
            gameObject.SetActive(false);
            foreach (var item in toDisableWhenActive)
            {
                item.SetActive(true);
            }
        }
        else
        {
            StartCoroutine(Error());
        }
        //else return an error that not everything has been set
    }

    IEnumerator Error()
    {
        errPanel.SetActive(true);
        float fluc = 0.25f;
        float flucMid = 0.5f;
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSecondsRealtime(0.15f);
            var mats = errPanel.GetComponent<Image>();
            fluc *= -1f;
            mats.color = new Color(mats.color.r, mats.color.r, mats.color.r, fluc + flucMid);

        }
        errPanel.SetActive(false);
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
        errPanel.SetActive(false);
        myInfos = GetValidProperties(this);
        customizable = true;
        CustomizationStart();
    }


}
