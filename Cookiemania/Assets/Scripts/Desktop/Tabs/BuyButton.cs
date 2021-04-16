using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour
{
    public UpgradeButton selectedUpgrade;

    private GameObject canvas = null;
    private TMPro.TextMeshProUGUI money = null;
    private TMPro.TextMeshProUGUI lvl = null;
    private Button button = null;

    private void Start()
    {
        button = GetComponent<Button>();
        canvas = GameObject.Find("ShopCanvas(Clone)");
        money = GameObject.Find("AnalyticsMoney").GetComponent<TMPro.TextMeshProUGUI>();
        lvl = GameObject.Find("AnalyticsLvl").GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        if (selectedUpgrade != null)
        {
            CheckCanBuy();
        }
        else
        {
            button.interactable = false;
        }
    }

    void CheckCanBuy()
    {
        if (selectedUpgrade.popupPrice > PlayerData.Player.money || selectedUpgrade.lvlReq > PlayerData.Player.shoplvl || selectedUpgrade.purchased == true)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }
    public void Buy()
    {
        selectedUpgrade.Buy();
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
