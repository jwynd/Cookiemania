using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Email_Click : MonoBehaviour
{
    [HideInInspector] public GameObject[] disable;
    [HideInInspector] public GameObject[] enable;
    [HideInInspector] public GameObject email;
    [HideInInspector] public string[] names;
    [HideInInspector] public UnityAction[] responses;
    [HideInInspector] public string subject;
    [HideInInspector] public string body;

    void OnMouseDown(){
        // Debug.Log("Clicked");
        GameObject e_go = Instantiate(email, Vector3.zero, Quaternion.identity);
        e_go.transform.SetParent(GameObject.Find("Canvas").transform, false);
        e_go.SetActive(true);
        Text s = e_go.transform.GetChild(1).GetComponent<Text>();
        s.text = subject;
        Text b = e_go.transform.GetChild(2).GetComponent<Text>();
        b.text = body;
        Email e = e_go.GetComponent<Email>();
        e.formatEmail(this.gameObject, disable, enable, names, responses);
        //email.formatEmail(this.gameObject, disable, enable, names, responses);
    }
    
    void OnDestroy(){
        foreach(GameObject d in disable){
            d.SetActive(true);
        }
        foreach(GameObject e in enable){
            Destroy(e);
        }
        Desktop_EmailController.Instance.reorderEmails();
    }
}
