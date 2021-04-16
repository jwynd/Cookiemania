using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public string popupText;
    public int lvlReq;
    public string popupTitle;
    public string popupQuote;
    public int popupPrice;
    public string purchaseClip = "upgrade_click";
    public bool purchased = false;
    private TMPro.TextMeshProUGUI[] upgradeTitles;
    private TMPro.TextMeshProUGUI[] upgradeDescriptions;
    private TMPro.TextMeshProUGUI[] upgradeCosts;
    private TMPro.TextMeshProUGUI[] upgradeQuotes;
    private BuyButton purchaseButton;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        upgradeTitles = ConvertToTextArr(GameObject.FindGameObjectsWithTag("UpgradeTitle"));
        upgradeDescriptions = ConvertToTextArr(GameObject.FindGameObjectsWithTag("UpgradeDescription"));
        upgradeCosts = ConvertToTextArr(GameObject.FindGameObjectsWithTag("UpgradeCost"));
        upgradeQuotes = ConvertToTextArr(GameObject.FindGameObjectsWithTag("UpgradeQuote"));
        purchaseButton = GameObject.FindGameObjectWithTag("Purchase").GetComponent<BuyButton>();
    }

    public TMPro.TextMeshProUGUI[] ConvertToTextArr(GameObject[] objects)
    {
        List<TMPro.TextMeshProUGUI> texts = new List<TMPro.TextMeshProUGUI>();
        foreach (var obj in objects)
        {
            var text = obj.GetComponent<TMPro.TextMeshProUGUI>();
            if (text != null)
                texts.Add(text);
        }
        return texts.ToArray();
    }

    public void select()
    {
        PlayerData.Player.userstats += 1;
        for (int i = 0; i < upgradeTitles.Length; i++)
        {
            upgradeTitles[i].text = popupTitle;
        }
        for (int i = 0; i < upgradeQuotes.Length; i++)
        {
            upgradeQuotes[i].text = popupQuote;
        }
        for (int i = 0; i < upgradeDescriptions.Length; i++)
        {
            upgradeDescriptions[i].text = popupText;
        }
        for (int i = 0; i < upgradeCosts.Length; i++)
        {
            upgradeCosts[i].text = "Cost: $" + popupPrice.ToString();
        }
        purchaseButton.selectedUpgrade = this;
    }
    public void Buy()
    {
        if (PlayerData.Player.money >= popupPrice &&
            PlayerData.Player.shoplvl >= lvlReq &&
            !purchased)
        {
            PlayerData.Player.money -= popupPrice;
            animator?.Play(purchaseClip, -1, 0f);
            GlobalReducer(gameObject.tag);
            CyberReducer(gameObject.tag);
            MarketingReducer(gameObject.tag);
            purchased = true;
        }
    }

    void CyberReducer(string tag)
    {
        switch (tag)
        {
            case "SHealth":
                PlayerData.Player.ShieldHealth += 1;
                break;
            case "SWidth":
                PlayerData.Player.ShieldWidth += 1;
                break;
            case "Pierce":
                PlayerData.Player.Pierce += 1;
                break;
            case "Spread":
                PlayerData.Player.GunSpread += 1;
                break;
            case "Iframes":
                PlayerData.Player.invulnerability += 1;
                break;
            case "Analyze":
                PlayerData.Player.SpaceUpgradelvl += 1;
                break;
            default:
                break;
        }
    }

    void GlobalReducer(string tag)
    {
        switch (tag)
        {
            case "milk":
                PlayerData.Player.healthlvl += 1;
                break;
            case "AI":
                PlayerData.Player.incomelvl += 1;
                break;
            default:
                break;
        }
    }

    void MarketingReducer(string type)
    {
        switch (type)
        {
            case "Jshield":
                PlayerData.Player.JShield += 1;
                break;
            case "Jcoinjump":
                PlayerData.Player.JCoinJump += 1;
                break;
            case "Jmagnet":
                PlayerData.Player.JMagnet += 1;
                break;
            case "Jjumppower":
                PlayerData.Player.JJumpPower += 1;
                break;
            case "Jmagnetcd":
                PlayerData.Player.JMagnetCD += 1;
                break;
            case "Jmagnetdistance":
                PlayerData.Player.JMagnetDistance += 1;
                break;
            default:
                break;
        }
    }
}
