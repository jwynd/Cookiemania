using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Flags f = Flags.Instance;
        Debug.Log("TEST 1 (get-nothing): "+(f.getFlag("Doesn'tExist")==-1?"PASS":"FAIL"));
        Debug.Log("TEST 2 (create-new): "+(f.createFlag("NewFlag", 0)?"PASS":"FAIL"));
        Debug.Log("TEST 3 (create-again): "+(!f.createFlag("NewFlag", 1)?"PASS":"FAIL"));
        Debug.Log("TEST 4 (get-exists): "+(f.getFlag("NewFlag")==0?"PASS":"FAIL"));
        Debug.Log("TEST 5 (set-exists): "+(f.setFlag("NewFlag", 1)?"PASS":"FAIL"));
        Debug.Log("TEST 5b (get-after-set): "+(f.getFlag("NewFlag")==1?"PASS":"FAIL"));
        Debug.Log("TEST 6 (set-nonexist-false): "+(!f.setFlag("Doesn'tExist", 21)?"PASS":"FAIL"));
        Debug.Log("TEST 6b (get-after-set-nonexist-false): "+(f.getFlag("Doesn'tExist")==-1?"PASS":"FAIL"));
        Debug.Log("TEST 7 (set-nonexist-true): "+(f.setFlag("Didn'tExist", 22, true)?"PASS":"FAIL"));
        Debug.Log("TEST 7b (get-after-set-nonexist-true): "+(f.getFlag("Didn'tExist")==22?"PASS":"FAIL"));
    }
}
