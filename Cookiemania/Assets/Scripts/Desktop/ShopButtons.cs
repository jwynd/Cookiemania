using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButtons : MonoBehaviour
{
    public int cost;
    string mini; //either cyber or marketing
    public int lvlReq;
    private bool purchased = false;
    public GameObject global;
    public GameObject marketing;
    public GameObject cyber;

    private void Start()
    {
        //if (PlayerData.Player.money >= cost && PlayerData.Player.shoplvl >= lvlReq && PlayerData.Player.SpaceUpgradelvl >= lvlReq && purchased == false)
       // {
        //    gameObject.GetComponent<Image>().color = Color.yellow;
        //}
    }

    private void Update()
    {
        if (purchased == true)
        {
            gameObject.GetComponent<Image>().color = Color.black;
        }
    }
    public void Buy()
    {
        if (PlayerData.Player.money >= cost && PlayerData.Player.shoplvl >= lvlReq && PlayerData.Player.SpaceUpgradelvl >= lvlReq && purchased == false)
        {
            PlayerData.Player.money -= cost;
            checkGlobal();
            checkCyber();
            purchased = true;
        }
    }

    void checkCyber()
    {
       if(gameObject.tag == "SHealth")
        {
            PlayerData.Player.ShieldHealth += 1;
            Debug.Log("player S Health lvl is now:"+ PlayerData.Player.ShieldHealth);
        } else if (gameObject.tag == "SWidth")
        {
            PlayerData.Player.ShieldWidth += 1;
            Debug.Log("player S Width lvl is now:" + PlayerData.Player.ShieldWidth);
        } else if (gameObject.tag == "Pierce")
        {
            PlayerData.Player.Pierce += 1;
            Debug.Log("player pierce lvl is now:" + PlayerData.Player.Pierce);
        } else if (gameObject.tag == "Spread")
        {
            PlayerData.Player.GunSpread += 1;
            Debug.Log("player GunSpread lvl is now:" + PlayerData.Player.GunSpread);
        } else if (gameObject.tag == "Iframes")
        {
            PlayerData.Player.invulnerability += 1;
            Debug.Log("player iframe lvl is now:" + PlayerData.Player.invulnerability);
        } else if (gameObject.tag == "Analyze")
        {
            PlayerData.Player.SpaceUpgradelvl += 1;
        } else
        {
            Debug.Log("Not a space purchase");
        }
    }

    void checkGlobal()
    {
        if(gameObject.tag == "milk")
        {
            PlayerData.Player.healthlvl += 1;
        }
    }

    public void Global()
    {
        global.SetActive(true);
        marketing.SetActive(false);
        cyber.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void Marketing()
    {
        global.SetActive(false);
        marketing.SetActive(true);
        cyber.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void Cyber()
    {
        global.SetActive(true);
        marketing.SetActive(false);
        cyber.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void Back()
    {
        if (this.gameObject.activeSelf != true)
        {
            global.SetActive(false);
            marketing.SetActive(false);
            cyber.SetActive(false);
            this.gameObject.SetActive(true);
        } else
        {
            //home tab?
        }
    }
}
