using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    // Start is called before the first frame update
    public static int cost;
    public string popupText;
    public int lvlReq;
    public string popupTitle;
    public string popupPrice;
    private bool purchased = false;
    private GameObject [] upgradeTitles;
    private GameObject [] upgradeDescriptions;
    private GameObject [] upgradeCosts;
    void Start()
    {
       upgradeTitles = GameObject.FindGameObjectsWithTag("UpgradeTitle");
       upgradeDescriptions = GameObject.FindGameObjectsWithTag("UpgradeDescription");
       upgradeCosts = GameObject.FindGameObjectsWithTag("UpgradeCost");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void select()
    {
        for (int i = 0; i < upgradeTitles.Length; i++)
        {
            upgradeTitles[i].GetComponent<TMPro.TextMeshProUGUI>().text = popupTitle;
        }
        for (int i = 0; i < upgradeDescriptions.Length; i++)
        {
            upgradeDescriptions[i].GetComponent<TMPro.TextMeshProUGUI>().text = popupText;
        }
        for (int i = 0; i < upgradeCosts.Length; i++)
        {
            upgradeCosts[i].GetComponent<TMPro.TextMeshProUGUI>().text = popupPrice;
        }
    }
    public void Buy()
    {
        if (PlayerData.Player.money >= cost && PlayerData.Player.shoplvl >= lvlReq && purchased == false)
        {
            PlayerData.Player.money -= cost;
            checkGlobal();
            //&& PlayerData.Player.SpaceUpgradelvl >= lvlReq
            checkCyber();
            //checkMarketing();
            purchased = true;
        }
    }

    void checkCyber()
    {
        if (gameObject.tag == "SHealth")
        {
            PlayerData.Player.ShieldHealth += 1;
            Debug.Log("player S Health lvl is now:" + PlayerData.Player.ShieldHealth);
        }
        else if (gameObject.tag == "SWidth")
        {
            PlayerData.Player.ShieldWidth += 1;
            Debug.Log("player S Width lvl is now:" + PlayerData.Player.ShieldWidth);
        }
        else if (gameObject.tag == "Pierce")
        {
            PlayerData.Player.Pierce += 1;
            Debug.Log("player pierce lvl is now:" + PlayerData.Player.Pierce);
        }
        else if (gameObject.tag == "Spread")
        {
            PlayerData.Player.GunSpread += 1;
            Debug.Log("player GunSpread lvl is now:" + PlayerData.Player.GunSpread);
        }
        else if (gameObject.tag == "Iframes")
        {
            PlayerData.Player.invulnerability += 1;
            Debug.Log("player iframe lvl is now:" + PlayerData.Player.invulnerability);
        }
        else if (gameObject.tag == "Analyze")
        {
            PlayerData.Player.SpaceUpgradelvl += 1;
        }
        else
        {
            Debug.Log("Not a space purchase");
        }
    }

    void checkGlobal()
    {
        if (gameObject.tag == "milk")
        {
            PlayerData.Player.healthlvl += 1;
        }
        if(gameObject.tag == "AI")
        {
            PlayerData.Player.incomelvl += 1;
        }
    }
}
