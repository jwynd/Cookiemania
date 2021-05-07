﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButtons : MonoBehaviour
{
    public int cost;
    public int lvlReq;
    [SerializeField]
    [Tooltip("Order matters, canvases should correlate to button names, homepage" +
        " is designated as the first canvas in this list")]
    protected List<Canvas> pages = new List<Canvas>();
    [SerializeField]
    protected Canvas upgradePopup = null;

    private GameObject canvas = null;
    private TMPro.TextMeshProUGUI money = null;
    private TMPro.TextMeshProUGUI lvl = null;

    private void Start()
    {
        canvas = GameObject.Find("ShopCanvas(Clone)");
        money = GameObject.Find("AnalyticsMoney").GetComponent<TMPro.TextMeshProUGUI>();
        lvl = GameObject.Find("AnalyticsLvl").GetComponent<TMPro.TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        OpenHomepage();
    }

    private void Update()
    {
        UpdatePlayer();
    }

    public void OpenHomepage()
    {
        OpenPage(0);
    }

    public void OpenPage(int pageNumber)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].enabled = i == pageNumber;
        }
        // upgrade popup is disabled for home page
        var atHome = pageNumber == 0;
        if (upgradePopup)
        {
            upgradePopup.enabled = !atHome;
        }
        // no need for the rest if on home page
        if (atHome)
        {
            return;
        }
        SetUpgradeButtonColor(pageNumber);
    }

    private void SetUpgradeButtonColor(int pageNumber)
    {
        if (pageNumber >= pages.Count || pageNumber < 0)
            return;
        var color = pages[pageNumber].GetComponentInChildren
                    <TMPro.TMP_Text>().color;
        // the child of the canvas's child
        // should have a button child
        var button = upgradePopup.transform.GetChild(0)?
            .GetComponentInChildren<Button>();
        if (button)
        {
            try
            {
                var child = button.transform.GetChild(0);
                // trying to change text color first as it's more 
                // visually important
                child.GetChild(0).GetComponent<TMPro.TMP_Text>().color = color;
                child.GetComponent<Image>().color = color;
            }
            catch (Exception e)
            {
                Debug.LogError("cannot change colors for upgrade button");
                Debug.LogError("expected image child of button with TMP text" +
                    " as child of image was not found");
                Debug.LogError(e);
            }
        }
    }

    public void UpdatePlayer()
    {
        if (canvas == null)
        {
            canvas = GameObject.Find("ShopCanvas(Clone)");
            money = GameObject.Find("AnalyticsMoney").GetComponent<TMPro.TextMeshProUGUI>();
            lvl = GameObject.Find("AnalyticsLvl").GetComponent<TMPro.TextMeshProUGUI>();
            if (money) money.text = "Money: $" + PlayerData.Player.money.ToString();
            if (lvl) lvl.text = "Lvl: " + PlayerData.Player.shoplvl.ToString();
        }
        else
        {
            money.text = "Money: $" + PlayerData.Player.money.ToString();
            lvl.text = "Lvl: " + PlayerData.Player.shoplvl.ToString();
        }
    }
}
