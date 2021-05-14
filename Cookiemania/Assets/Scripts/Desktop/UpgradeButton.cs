using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    private const string purchaseClip = "upgrade_click";
    private const string connectorClip = "upgrade_connector";

    [System.Flags]
    [System.Serializable]
    public enum UpgradeType
    {
        SShieldHP = 1 << 0,
        SShieldWidth = 1 << 1,
        SPierce = 1 << 2,
        SSpread = 1 << 3,
        SInvulnerability = 1 << 4,
        GHealth = 1 << 5,
        GAI = 1 << 6,
        JShield = 1 << 7,
        JCoinJump = 1 << 8,
        JJumpPower = 1 << 9,
        JMagnet = 1 << 10,
        JMagnetCD = 1 << 11,
        JMagnetDistance = 1 << 12,
        JumperUpgrade = JShield | JCoinJump | JJumpPower | JMagnet | JMagnetCD | JMagnetDistance,
        SpaceUpgrade = SShieldHP | SShieldWidth | SPierce | SSpread | SInvulnerability,
        GlobalUpgrade = GHealth | GAI,
    }

    public UpgradeType upgradeType;
    public string popupText;
    public UpgradeButton[] precedingUpgrades;
    [SerializeField]
    private int _UID = -1;
    public int UID { private set { _UID = value; } get { return _UID; } }
    public string popupTitle;
    public string popupQuote;
    public int popupPrice;
    public Animator[] connectors;
    public bool Purchased
    {
        private set
        {
            _purchased = value;
            if (_purchased == false) return;
            gameObject.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 1f);
        }

        get { return _purchased; }
    }
    public bool CanPurchase
    {
        get 
        { 
            if (PlayerData.Player.money < popupPrice || Purchased) 
                return false;
            foreach (var button in precedingUpgrades)
            {
                if (!button.Purchased) return false;
            }
            return true;
        }
    }
    private bool _purchased = false;
    private TMPro.TextMeshProUGUI[] upgradeTitles;
    private TMPro.TextMeshProUGUI[] upgradeDescriptions;
    private TMPro.TextMeshProUGUI[] upgradeCosts;
    private TMPro.TextMeshProUGUI[] upgradeQuotes;
    private BuyButton purchaseButton;
    private Animator animator;


    void Start()
    {
#if UNITY_EDITOR
        var allUpgradeButtons = FindObjectsOfType<UpgradeButton>();
        HashSet<int> ids = new HashSet<int>();
        foreach(var button in allUpgradeButtons)
        {
            if (ids.Contains(button.UID)) 
                throw new System.Exception("matching unique ids on upgrade buttons: " +
                    button.popupTitle);
            ids.Add(button.UID);
        }
#endif
        Purchased = LoadedGame();
        animator = GetComponent<Animator>();
        upgradeTitles = ConvertToTextArr(GameObject.FindGameObjectsWithTag("UpgradeTitle"));
        upgradeDescriptions = ConvertToTextArr(GameObject.FindGameObjectsWithTag("UpgradeDescription"));
        upgradeCosts = ConvertToTextArr(GameObject.FindGameObjectsWithTag("UpgradeCost"));
        upgradeQuotes = ConvertToTextArr(GameObject.FindGameObjectsWithTag("UpgradeQuote"));
        purchaseButton = GameObject.FindGameObjectWithTag("Purchase").GetComponent<BuyButton>();
    }

    private bool LoadedGame()
    {
        if (SaveSystem.DontLoad()) return false;
        // if this is a purchased upgrade, it's upgrade levels were recorded in the save game already
        if (PlayerData.Player.UpgradesPurchased.TryGetValue(UID, out bool bought))
            return bought;
        return false;
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
            upgradeCosts[i].text = Purchased ? "Unlocked!" : "Cost: $" + popupPrice.ToString();
        }
        purchaseButton.selectedUpgrade = this;
    }
    public void Buy()
    {
        if (CanPurchase)
        {
            PlayerData.Player.userstats += 1;
            PlayerData.Player.money -= popupPrice;
            animator?.Play(purchaseClip, -1, 0f);
            foreach (var anim in connectors)
            {
                anim.Play(connectorClip, -1, 0f);
            }
            GlobalReducer(upgradeType);
            CyberReducer(upgradeType);
            MarketingReducer(upgradeType);
            Purchased = true;
            PlayerData.Player.UpgradesPurchased.Add(UID, true);
            for (int i = 0; i < upgradeCosts.Length; i++)
            {
                upgradeCosts[i].text = "Unlocked!";
            }
        }
    }

    void CyberReducer(UpgradeType type)
    {
        if ((type & UpgradeType.SpaceUpgrade) == 0) return;
        Debug.LogWarning(type.ToString() + " has a space upgrade");
        if (type.HasFlag(UpgradeType.SShieldHP))
        {
            PlayerData.Player.ShieldHealth += 1;
        }
        if (type.HasFlag(UpgradeType.SShieldWidth))
        {
            PlayerData.Player.ShieldWidth += 1;
        }
        if (type.HasFlag(UpgradeType.SPierce))
        {
            PlayerData.Player.Pierce += 1;
        }
        if (type.HasFlag(UpgradeType.SSpread))
        {
            PlayerData.Player.GunSpread += 1;
        }
        if (type.HasFlag(UpgradeType.SInvulnerability))
        {
            PlayerData.Player.invulnerability += 1;
        }
    }

    void GlobalReducer(UpgradeType type)
    {
        if ((type & UpgradeType.GlobalUpgrade) == 0) return;
        Debug.LogWarning(type.ToString() + " has a global upgrade");
        if (type.HasFlag(UpgradeType.GHealth))
        {
            PlayerData.Player.healthlvl += 1;
        }
        if (type.HasFlag(UpgradeType.GAI))
        {
            PlayerData.Player.ai += 1;
        }
    }

    void MarketingReducer(UpgradeType type)
    {
        if ((type & UpgradeType.JumperUpgrade) == 0) return;
        Debug.LogWarning(type.ToString() + " has a jumper upgrade");
        if (type.HasFlag(UpgradeType.JShield))
        {
            PlayerData.Player.JShield += 1;
        }
        if (type.HasFlag(UpgradeType.JMagnetCD))
        {
            PlayerData.Player.JMagnetCD += 1;
        }
        if (type.HasFlag(UpgradeType.JCoinJump))
        {
            PlayerData.Player.JCoinJump += 1;
        }
        if (type.HasFlag(UpgradeType.JMagnet))
        {
            PlayerData.Player.JMagnet += 1;
        }
        if (type.HasFlag(UpgradeType.JJumpPower))
        {
            PlayerData.Player.JJumpPower += 1;
        }
        if (type.HasFlag(UpgradeType.JMagnetDistance))
        {
            PlayerData.Player.JMagnetDistance += 1;
        }
    }
}
