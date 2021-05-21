﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterContentManager : MonoBehaviour
{
    enum AppearanceDetails
    {
        BODY,
        TOPPINGS,
        TOPACCESSORY,
        MIDDLEACCESSORY,
        BOTTOMACCESSORY,
        EYESHAPE,
        EYEBROWSHAPE
    }



    [SerializeField] private GameObject[] bodys = null;
    [SerializeField] private Transform bodyAnchor = null;
    [SerializeField] private GameObject[] toppings = null;
    [SerializeField] private Transform ToppingAnchor = null;
    [SerializeField] private GameObject[] TAccess = null;
    [SerializeField] private Transform TopAnchor = null;
    [SerializeField] private GameObject[] MAccess = null;
    [SerializeField] private Transform MidAnchor = null;
    [SerializeField] private GameObject[] BAccess = null;
    [SerializeField] private Transform BotAnchor = null;
    [SerializeField] private GameObject[] Eyes = null;
    [SerializeField] private Transform EyeAnchor = null;
    [SerializeField] private GameObject[] Eyebrows = null;
    [SerializeField] private Transform BrowAnchor = null;
    [SerializeField] private GameObject newChara = null;
    [SerializeField] private Transform GlassesAnchor = null;
    [SerializeField] private Transform HeadphonesAnchor = null;
    [SerializeField] private Transform ScarfAnchor = null;
    GameObject activebody;
    GameObject activeTopping;
    GameObject activeTop;
    GameObject activeMid;
    GameObject activeBot;
    GameObject activeEye;
    GameObject activeBrow;

    public GameObject characterSprite { get; protected set; }
    public static CharacterContentManager Instance { get; protected set; }
    public string characterName { get; protected set; } = null;
    public string companyName { get; protected set; } = null;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject panelName;

    int bodyindex = 0;
    int toppingindex = 0;
    int topindex = 0;
    int midindex = 0;
    int botindex = 0;
    int eyeindex = 0;
    int browindex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
    private void Start()
    {
        ApplyCustom(AppearanceDetails.BODY, 0);
        ApplyCustom(AppearanceDetails.TOPPINGS, 0);
        ApplyCustom(AppearanceDetails.TOPACCESSORY, 0);
        ApplyCustom(AppearanceDetails.MIDDLEACCESSORY, 0);
        ApplyCustom(AppearanceDetails.BOTTOMACCESSORY, 0);
        ApplyCustom(AppearanceDetails.EYESHAPE, 0);
        ApplyCustom(AppearanceDetails.EYEBROWSHAPE, 0);
        panel.SetActive(false);
        var temp = panel.GetComponent<ConfimationUI>();
        temp.SetCancel(cancel);
        temp.SetConfirm(confirm);
        characterSprite = newChara;
        characterSprite.SetActive(false);
        panelName.SetActive(false);

        if (SaveSystem.DontLoad()) return;
        //TODO: alright need to set up the character from the saved game
        // probably gonna be a dictionary of enum to int values?
        // iterate through dictionary and select stuff
        // then finalize with confirm
        characterName = PlayerData.Player.Name;
        companyName = PlayerData.Player.CompanyName;
        GetFromPlayerData();
        confirm();
    }



    public void bodyup()
    {
        if (bodyindex < bodys.Length - 1)
            bodyindex++;
        else
            bodyindex = 0;
        ApplyCustom(AppearanceDetails.BODY, bodyindex);
    }

    public void bodydown()
    {
        if (bodyindex > 0)
            bodyindex--;
        else
            bodyindex = bodys.Length - 1;
        ApplyCustom(AppearanceDetails.BODY, bodyindex);
    }
    public void toppingup()
    {
        if (toppingindex < toppings.Length - 1)
            toppingindex++;
        else
            toppingindex = 0;
        ApplyCustom(AppearanceDetails.TOPPINGS, toppingindex);
    }

    public void toppingdown()
    {
        if (toppingindex > 0)
            toppingindex--;
        else
            toppingindex = toppings.Length - 1;
        ApplyCustom(AppearanceDetails.TOPPINGS, toppingindex);
    }
    public void topup()
    {
        if (topindex < TAccess.Length - 1)
            topindex++;
        else
            topindex = 0;
        ApplyCustom(AppearanceDetails.TOPACCESSORY, topindex);
    }

    public void topdown()
    {
        if (topindex > 0)
            topindex--;
        else
            topindex = TAccess.Length - 1;
        ApplyCustom(AppearanceDetails.TOPACCESSORY, topindex);
    }
    public void midup()
    {
        if (midindex < MAccess.Length - 1)
            midindex++;
        else
            midindex = 0;
        ApplyCustom(AppearanceDetails.MIDDLEACCESSORY, midindex);
    }

    public void middown()
    {
        if (midindex > 0)
            midindex--;
        else
            midindex = MAccess.Length - 1;
        ApplyCustom(AppearanceDetails.MIDDLEACCESSORY, midindex);
    }
    public void botup()
    {
        if (botindex < BAccess.Length - 1)
            botindex++;
        else
            botindex = 0;
        ApplyCustom(AppearanceDetails.BOTTOMACCESSORY, botindex);
    }

    public void botdown()
    {
        if (botindex > 0)
            botindex--;
        else
            botindex = BAccess.Length - 1;
        ApplyCustom(AppearanceDetails.BOTTOMACCESSORY, botindex);
    }

    public void eyeup()
    {
        if (eyeindex < Eyes.Length - 1)
            eyeindex++;
        else
            eyeindex = 0;
        ApplyCustom(AppearanceDetails.EYESHAPE, eyeindex);
    }

    public void eyedown()
    {
        if (eyeindex > 0)
            eyeindex--;
        else
            eyeindex = Eyes.Length - 1;
        ApplyCustom(AppearanceDetails.EYESHAPE, eyeindex);
    }
    public void browup()
    {
        if (browindex < Eyebrows.Length - 1)
            browindex++;
        else
            browindex = 0;
        ApplyCustom(AppearanceDetails.EYEBROWSHAPE, browindex);
    }

    public void browdown()
    {
        if (browindex > 0)
            browindex--;
        else
            browindex = Eyebrows.Length - 1;
        ApplyCustom(AppearanceDetails.EYEBROWSHAPE, browindex);
    }

    //Application start
    void ApplyCustom(AppearanceDetails detail, int id)
    {
        switch (detail)
        {
            case AppearanceDetails.BODY:
                if (activebody != null)
                {
                    Destroy(activebody);
                }
                activebody = Instantiate(bodys[id]);
                activebody.transform.SetParent(bodyAnchor);
                activebody.transform.ResetTransform();
                break;

            case AppearanceDetails.TOPPINGS:
                if (activeTopping != null)
                {
                    Destroy(activeTopping);
                }
                activeTopping = Instantiate(toppings[id]);
                activeTopping.transform.SetParent(ToppingAnchor);
                activeTopping.transform.ResetTransform();
                break;

            case AppearanceDetails.TOPACCESSORY:
                if (activeTop != null)
                {
                    Destroy(activeTop);
                }
                activeTop = Instantiate(TAccess[id]);
                if (id == 2)
                {
                    activeTop.transform.SetParent(HeadphonesAnchor);
                    activeTop.transform.ResetTransform();
                }
                else
                {
                    activeTop.transform.SetParent(TopAnchor);
                    activeTop.transform.ResetTransform();
                }
                break;

            case AppearanceDetails.MIDDLEACCESSORY:
                if (activeMid != null)
                {
                    Destroy(activeMid);
                }
                activeMid = Instantiate(MAccess[id]);
                if (id == 2)
                {
                    activeMid.transform.SetParent(GlassesAnchor);
                    activeMid.transform.ResetTransform();
                }
                else
                {
                    activeMid.transform.SetParent(MidAnchor);
                    activeMid.transform.ResetTransform();
                }
                break;

            case AppearanceDetails.BOTTOMACCESSORY:
                if (activeBot != null)
                {
                    Destroy(activeBot);
                }
                activeBot = Instantiate(BAccess[id]);
                if (id == 3)
                {
                    activeBot.transform.SetParent(ScarfAnchor);
                    activeBot.transform.ResetTransform();
                }
                else
                {
                    activeBot.transform.SetParent(BotAnchor);
                    activeBot.transform.ResetTransform();
                }
                break;

            case AppearanceDetails.EYESHAPE:
                if (activeEye != null)
                {
                    Destroy(activeEye);
                }
                activeEye = Instantiate(Eyes[id]);
                activeEye.transform.SetParent(EyeAnchor);
                activeEye.transform.ResetTransform();
                break;

            case AppearanceDetails.EYEBROWSHAPE:
                if (activeBrow != null)
                {
                    Destroy(activeBrow);
                }
                activeBrow = Instantiate(Eyebrows[id]);
                activeBrow.transform.SetParent(BrowAnchor);
                activeBrow.transform.ResetTransform();
                break;

        }

    }

    public void dialoguebox()
    {
        if (characterName == null || characterName.Length < 2 ||
            companyName == null || companyName.Length < 2)
        {
            panelName.SetActive(true);
            return;
        }
        Debug.Log("dialogue");
        panel.SetActive(true);
    }

    public void cancel()
    {
        Debug.Log("closed");
        panel.SetActive(false);
    }

    public void confirm()
    {
        //to do reconstruct character
        gameObject.SetActive(false);
        var temp = characterSprite.GetComponent<CharPrefab>();
        temp.body = bodys[bodyindex].GetComponent<UnityEngine.UI.Image>().sprite;
        temp.topping = toppings[toppingindex].GetComponent<UnityEngine.UI.Image>().sprite;
        temp.top = TAccess[topindex].GetComponent<UnityEngine.UI.Image>().sprite;
        temp.middle = MAccess[midindex].GetComponent<UnityEngine.UI.Image>().sprite;
        temp.bottom = BAccess[botindex].GetComponent<UnityEngine.UI.Image>().sprite;
        temp.eyes = Eyes[eyeindex].GetComponent<UnityEngine.UI.Image>().sprite;
        temp.eyebrows = Eyebrows[browindex].GetComponent<UnityEngine.UI.Image>().sprite;
        temp.name = characterName;
        temp.CharName = characterName;
        temp.CompanyName = companyName;

        SetOnPlayer();
        /*foreach (Transform child in temp.transform)
        {
            child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y, 0);
            foreach (Transform grandchild in child.transform)
            {
                grandchild.localPosition = new Vector3(grandchild.localPosition.x, grandchild.localPosition.y, 0);

            }
        }*/
        temp.transform.position = new Vector3(1000, 1000, 0);
        temp.gameObject.SetActive(true);
    }

    private void GetFromPlayerData()
    {
        if (PlayerData.Player.CustomizeData.Count < 7) return;
        bodyindex = PlayerData.Player.CustomizeData[0];
        toppingindex = PlayerData.Player.CustomizeData[1];
        topindex = PlayerData.Player.CustomizeData[2];
        midindex = PlayerData.Player.CustomizeData[3];
        botindex = PlayerData.Player.CustomizeData[4];
        eyeindex = PlayerData.Player.CustomizeData[5];
        browindex = PlayerData.Player.CustomizeData[6];
    }

    private void SetOnPlayer()
    {
        // we're just adding in order from our indices
        var spriteInts = new List<int>() { bodyindex, toppingindex, topindex, midindex, botindex, eyeindex, browindex };
        PlayerData.Player.CustomizeData = spriteInts;
    }

    public void okay()
    {
        panelName.SetActive(false);
    }

    public void SetName(string name)
    {
        characterName = name;

    }

    public void SetCompanyName(string name)
    {
        companyName = name;
    }


}