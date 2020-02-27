using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Email_Click : MonoBehaviour
{
    public GameObject[] disable;
    public GameObject[] enable;
    public Email email;
    private void adjustMoney(){
        General_Score.Instance.money += 10;
    }
    private void adjustTrust(){
        General_Score.Instance.trust += 21;
    }
    private void adjustBoth(){
        adjustMoney();
        adjustTrust();
    }
    
    void Awake(){

    }

    void OnMouseDown(){
        // Debug.Log("Clicked");
        email.formatEmail(this.gameObject, disable, enable,new string[] {"Money", "Trust", "Both"} ,new UnityAction[] {adjustMoney, adjustTrust, adjustBoth});
    }
    
    void OnDestroy(){
        foreach(GameObject d in disable){
            d.SetActive(true);
        }
        foreach(GameObject e in enable){
            Destroy(e);
        }
    }
}
