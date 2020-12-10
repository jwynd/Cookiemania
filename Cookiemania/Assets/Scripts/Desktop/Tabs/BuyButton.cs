using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour
{
    public UpgradeButton selectedUpgrade;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(selectedUpgrade != null)
        {
            checkactive();
        } else
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
    }

    void checkactive()
    {
        if (selectedUpgrade.popupPrice > PlayerData.Player.money || selectedUpgrade.lvlReq > PlayerData.Player.shoplvl || selectedUpgrade.purchased == true)
        {
            gameObject.GetComponent<Button>().interactable = false;
        } else
        {
            gameObject.GetComponent<Button>().interactable = true;
        }
    }
    public void Buy()
    {
        selectedUpgrade.Buy();
    }
    public void UpdatePlayer()
    {
        if (GameObject.Find("ShopCanvas(Clone)") != null)
        {
            GameObject.Find("AnalyticsMoney").GetComponent<TMPro.TextMeshProUGUI>().text = "Money: $" + PlayerData.Player.money.ToString();
            GameObject.Find("AnalyticsLvl").GetComponent<TMPro.TextMeshProUGUI>().text = "Lvl: " + PlayerData.Player.shoplvl.ToString();
        }

    }
}
