using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Email_Click : MonoBehaviour
{
    public GameObject[] disable;
    public GameObject[] enable;
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
    void OnMouseDown(){
        // Debug.Log("Clicked");
        Email_Example.Instance.formatEmail(disable, enable,new string[] {"Money", "Trust", "Both"} ,new Email_Example.response[] {adjustMoney, adjustTrust, adjustBoth});
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
