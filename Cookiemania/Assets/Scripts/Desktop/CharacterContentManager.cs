using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private GameObject[] bodys;
    [SerializeField] private Transform bodyAnchor;
    [SerializeField] private GameObject[] toppings;
    [SerializeField] private Transform ToppingAnchor;
    [SerializeField] private GameObject[] TAccess;
    [SerializeField] private Transform TopAnchor;
    [SerializeField] private GameObject[] MAccess;
    [SerializeField] private Transform MidAnchor;
    [SerializeField] private GameObject[] BAccess;
    [SerializeField] private Transform BotAnchor;
    [SerializeField] private GameObject[] Eyes;
    [SerializeField] private Transform EyeAnchor;
    [SerializeField] private GameObject[] Eyebrows;
    [SerializeField] private Transform BrowAnchor;
    GameObject activebody;
    GameObject activeTopping;
    GameObject activeTop;
    GameObject activeMid;
    GameObject activeBot;
    GameObject activeEye;
    GameObject activeBrow;

    int bodyindex = 0;
    int toppingindex = 0;
    int topindex = 0;
    int midindex = 0;
    int botindex = 0;
    int eyeindex = 0;
    int browindex = 0;

    private void Start()
    {
        ApplyCustom(AppearanceDetails.BODY, 0);
        ApplyCustom(AppearanceDetails.TOPPINGS, 0);
        ApplyCustom(AppearanceDetails.TOPACCESSORY, 0);
        ApplyCustom(AppearanceDetails.MIDDLEACCESSORY, 0);
        ApplyCustom(AppearanceDetails.BOTTOMACCESSORY, 0);
        ApplyCustom(AppearanceDetails.EYESHAPE, 0);
        ApplyCustom(AppearanceDetails.EYEBROWSHAPE, 0);
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
        if(bodyindex > 0)
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
                if(activebody != null)
                {
                    GameObject.Destroy(activebody);
                }
                activebody = GameObject.Instantiate(bodys[id]);
                activebody.transform.SetParent(bodyAnchor);
                activebody.transform.ResetTransform();
                break;
            
            case AppearanceDetails.TOPPINGS:
                if (activeTopping != null)
                {
                    GameObject.Destroy(activeTopping);
                }
                activeTopping = GameObject.Instantiate(toppings[id]);
                activeTopping.transform.SetParent(ToppingAnchor);
                activeTopping.transform.ResetTransform();
                break;
           
            case AppearanceDetails.TOPACCESSORY:
                if (activeTop != null)
                {
                    GameObject.Destroy(activeTop);
                }
                activeTop = GameObject.Instantiate(TAccess[id]);
                activeTop.transform.SetParent(TopAnchor);
                activeTop.transform.ResetTransform();
                break;

            case AppearanceDetails.MIDDLEACCESSORY:
                if (activeMid != null)
                {
                    GameObject.Destroy(activeMid);
                }
                activeMid = GameObject.Instantiate(MAccess[id]);
                activeMid.transform.SetParent(MidAnchor);
                activeMid.transform.ResetTransform();
                break;

            case AppearanceDetails.BOTTOMACCESSORY:
                if (activeBot != null)
                {
                    GameObject.Destroy(activeBot);
                }
                activeBot = GameObject.Instantiate(BAccess[id]);
                activeBot.transform.SetParent(BotAnchor);
                activeBot.transform.ResetTransform();
                break;

            case AppearanceDetails.EYESHAPE:
                if (activeEye != null)
                {
                    GameObject.Destroy(activeEye);
                }
                activeEye = GameObject.Instantiate(Eyes[id]);
                activeEye.transform.SetParent(EyeAnchor);
                activeEye.transform.ResetTransform();
                break;

        }
    }

}
