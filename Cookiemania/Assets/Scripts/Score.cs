using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [HideInInspector] public int money = 100;
    [HideInInspector] public int trust = 0;
    private Text text;
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        text = this.transform.GetChild(0).gameObject.GetComponent<Text>();
        image = this.transform.GetChild(1).gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "$"+money;
        if(Input.GetKeyDown(KeyCode.M)){
            money += (int)Mathf.Floor((Random.value*10)-5);
        }
        if(Input.GetKeyDown(KeyCode.T)){
            trust += (int)Mathf.Floor(Random.value*5);
        }
        if(trust < 20){
            image.color = Color.black;
        } else if(trust < 40){
            image.color = Color.red;
        } else if(trust < 60){
            image.color = Color.green;
        } else {
            image.color = Color.white;
        }
    }
}
