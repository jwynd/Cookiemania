using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabButton : MonoBehaviour
{
    public bool homeTab = false;
    public GameObject[] otherTabs;
    // Start is called before the first frame update
    void Start()
    {
        if(homeTab)deactivateOthers();
    }

    public void click(){
        if(!this.gameObject.activeSelf){
            this.gameObject.SetActive(true);
            deactivateOthers();
        }
    }

    private void deactivateOthers(){
        foreach(GameObject g in otherTabs){
            g.SetActive(false);
        }
    }
}
