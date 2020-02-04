using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Test_PlatformTrigger : MonoBehaviour
{
    public Test_PlatformGeneration gen;
    
    void OnTriggerEnter2D(Collider2D other){
        gen.BuildSection();
        // Debug.Log("Trigger Entered");
        if(this.gameObject.name != gen.trigger.name) Destroy(this.gameObject);
    }
}
