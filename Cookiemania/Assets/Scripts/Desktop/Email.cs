using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Email : MonoBehaviour
{
    private GameObject[] g_enable;
    private GameObject[] g_disable;
    private GameObject source;
    
    void Awake(){
        gameObject.SetActive(false);
    }

    public void formatEmail(GameObject spawner, GameObject[] disable, GameObject[] enable, string[] names, UnityAction[] rs){
        foreach(GameObject d in disable){
            d.SetActive(false);
        }
        foreach(GameObject e in enable){
            e.SetActive(true);
        }
        g_enable = disable;
        g_disable = enable;
        source = spawner;
        formatResponse(names, rs);
    }

    public void formatResponse(string[] names, UnityAction[] rs){
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
            Buttons[i].onClick.AddListener(rs[i]);
            Buttons[i].onClick.AddListener(respondAndDelete);
        }
    }

    void OnDestroy(){
        foreach(GameObject g in g_enable){
            g.SetActive(true);
        }
        foreach(GameObject g in g_disable){
            g.SetActive(false);
        }
        Destroy(source);
    }
    
    private void respondAndDelete(){
        Destroy(this.gameObject);
    }
}
