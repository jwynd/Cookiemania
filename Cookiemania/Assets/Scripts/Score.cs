using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [HideInInspector] public int money = 100;
    [HideInInspector] public int trust = 0;
    private Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = this.gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "$"+money;
        if(Input.GetKeyDown(KeyCode.A)){
            money += (int)Mathf.Floor((Random.value*10)-5);
        }
    }
}
