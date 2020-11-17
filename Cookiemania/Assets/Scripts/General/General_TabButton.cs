using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Tracking.LocationUtils;

public class General_TabButton : MonoBehaviour
{
    public bool homeTab = false;
    public GameObject[] otherTabs;
    public Locale myLocation;
    private void Awake()
    {
        if (!myLocation.IsDesktop())
        {
            Debug.LogError("invalid location, tabs must have a desktop " +
                "defined location");
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (var tabObj in otherTabs)
        {
            var tab = tabObj.GetComponent<General_TabButton>();
            if (tab.myLocation == myLocation)
            {
                Debug.LogError("other tabs may not have my tab locale" +
                    " designation: " + myLocation);
            }
        }
        if (homeTab)deactivateOthers();
    }

    public void click(){
        if(!this.gameObject.activeSelf){
            this.gameObject.SetActive(true);
            if (PlayerData.Player)
            {
                PlayerData.Player.Location.Current = myLocation;
            }
            deactivateOthers();
            this.gameObject.tag = "Untagged";
        }
    }

    private void deactivateOthers(){
        foreach(GameObject g in otherTabs){
            g.SetActive(false);
            g.tag = "DeactivateOnLoad";
        }
    }
}
