using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    float rotatespeed = 3f;
    public float sensitivity;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("q"))
            //print ("q key was pressed");
            this.transform.Rotate(Vector3.forward * rotatespeed);
        if (Input.GetKey("e"))
            //print ("e key was pressed");
            this.transform.Rotate(Vector3.forward * -rotatespeed);

    }
}
