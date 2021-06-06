using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    float rotatespeed = 1f;
    public float sensitivity;
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        var rotation = InputAxes.Instance.Rotation.ReadValue<float>();
        if (rotation > 0f)
            this.transform.Rotate(Vector3.forward * rotatespeed);
        else if (rotation < 0f)
            this.transform.Rotate(Vector3.forward * -rotatespeed);

    }
}
