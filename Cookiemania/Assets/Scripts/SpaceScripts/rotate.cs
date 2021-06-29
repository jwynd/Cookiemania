using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    float rotatespeed = 500f;
    public float sensitivity;

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        var rotation = InputAxes.Instance.Rotation.ReadValue<float>();
        if (rotation > 0f)
            transform.Rotate(Vector3.forward * rotatespeed * Time.deltaTime);
        else if (rotation < 0f)
            transform.Rotate(Vector3.forward * -rotatespeed * Time.deltaTime);

    }
}
