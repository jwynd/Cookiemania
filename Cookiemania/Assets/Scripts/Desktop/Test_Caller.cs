using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Caller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void call1(){
        Debug.Log("Calling option 1");
    }

    private void call2(){
        Debug.Log("Calling option 2");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)){
            Test_Callback.Instance.callback(call1, call2);
        }
    }
}
