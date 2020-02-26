using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Email_Example : MonoBehaviour
{
    public static Email_Example Instance;
    public delegate void response();
    void Awake(){
        if(Instance != null && Instance != this){
            Destroy(this);
        }
        Instance = this;
        gameObject.SetActive(false);
    }
    public void formatEmail(GameObject[] disable, GameObject[] enable, string[] names, response[] rs){
        foreach(GameObject d in disable){
            d.SetActive(false);
        }
        foreach(GameObject e in enable){
            e.SetActive(true);
        }
        formatResponse(names, rs);
    }

    public void formatResponse(string[] names, response[] rs){
        // format response based on length of rs
        float yOffset = -80.0f;
        Button[] Buttons = new Button[rs.Length];
        for(int i = 0; i < rs.Length; ++i){
            GameObject g = new GameObject();
            g.transform.position = new Vector3(Screen.width *0.5f, (Screen.height *0.5f) + yOffset, 0.0f);
            g.transform.parent = this.gameObject.transform;
            g.AddComponent<Image>();
            // RectTransform rt = g.GetComponent<RectTransform>();
            GameObject c = new GameObject();
            c.transform.parent = g.transform;
            c.transform.position = g.transform.position;
            Text txt = c.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            txt.color = Color.black;
            txt.text = names[i];
            yOffset -= 20.0f;
            Buttons[i] = g.AddComponent<Button>();
            Buttons[i].onClick.AddListener((UnityAction)rs[i]);
        }
    }
}
