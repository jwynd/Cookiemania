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
    [SerializeField]
    protected GameObject global = null;
    [SerializeField]
    protected GameObject marketing = null;
    [SerializeField]
    protected GameObject cyber = null;

    protected Canvas controlledCanvas1;
    protected Canvas controlledCanvas2;
    protected Canvas controlledCanvas3;
    protected Canvas controlledCanvas4;
    protected GameObject controlledObj1;
    protected GameObject controlledObj2;
    protected GameObject controlledObj3;
    protected GameObject controlledObj4;

    private void Awake()
    {
        controlledObj4 = GameObject.Find("ShopCanvas(Clone)");
        controlledCanvas4 = controlledObj4.GetComponent<Canvas>();
        if (gameObject.name == "ShopCanvas(Clone)")
        {
            controlledObj1 = Instantiate(global);
            controlledCanvas1 = controlledObj1.GetComponent<Canvas>();         

            controlledObj2 = Instantiate(marketing);
            controlledCanvas2 = controlledObj2.GetComponent<Canvas>();        

            controlledObj3 = Instantiate(cyber);
            controlledCanvas3 = controlledObj3.GetComponent<Canvas>();

            Debug.Log("Is Shop Canvas");
        }
        else
        {
            Debug.Log("NotShop Canvas");
        }              
    }
    private void Start()
    {
        if (gameObject.name == "ShopCanvas(Clone)")
        {
            controlledObj1.transform.SetParent(SiteCanvas.Instance.transform);
            controlledObj2.transform.SetParent(SiteCanvas.Instance.transform);
            controlledObj3.transform.SetParent(SiteCanvas.Instance.transform);
        }
        if (gameObject.name != "ShopCanvas(Clone)")
        {
            controlledObj1 = GameObject.Find("Global(Clone)");
            controlledCanvas1 = controlledObj1.GetComponent<Canvas>();
            controlledObj2 = GameObject.Find("Marketing(Clone)");
            controlledCanvas2 = controlledObj2.GetComponent<Canvas>();
            controlledObj3 = GameObject.Find("Cyber(Clone)");
            controlledCanvas3 = controlledObj3.GetComponent<Canvas>();
        }
        if (GameObject.Find("Cyber(Clone)"))
        {
            controlledCanvas1.enabled = false;
            controlledCanvas2.enabled = false;
            controlledCanvas3.enabled = false;
        }
    }

    private void Update()
    {
    }

    public void Global()
    {
        controlledCanvas1.enabled = true;
        controlledCanvas2.enabled = false;
        controlledCanvas3.enabled = false;
        controlledCanvas4.enabled = false;
    }

    public void Marketing()
    {
        controlledCanvas1.enabled = false;
        controlledCanvas2.enabled = true;
        controlledCanvas3.enabled = false;
        controlledCanvas4.enabled = false;
    }

    public void Cyber()
    {
        controlledCanvas1.enabled = false;
        controlledCanvas2.enabled = false;
        controlledCanvas3.enabled = true;
        controlledCanvas4.enabled = false;
    }

    public void Back()
    {    
            controlledCanvas1.enabled = false;
            controlledCanvas2.enabled = false;
            controlledCanvas3.enabled = false;
            controlledCanvas4.enabled = true;
    }
}
