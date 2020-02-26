using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Callback : MonoBehaviour
{
    public delegate void Del();
    public static Test_Callback Instance;
    void Awake(){
        if(Instance != null && Instance != this){
            Destroy(this);
        }
        Instance = this;
    }
    // Start is called before the first frame update
    public void callback(Del handler1, Del handler2){
        // test the callback stuff
        Debug.Log("Before Callback");
        float f = Random.Range(0.0f, 1.0f);
        if(f > 0.5f) handler1();
        else handler2();
        Debug.Log("After Callback");
    }
}
