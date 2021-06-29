using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unflipper : MonoBehaviour
{
    // Start is called before the first frame update
    private SpriteRenderer parentRend;
    private float lastFlip = 1f;
    void Start()
    {
        parentRend = transform.parent.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (parentRend.transform.localScale.x * lastFlip < 0f)
        {
            lastFlip = parentRend.transform.localScale.x;
            transform.localScale = new Vector3(
                -transform.localScale.x, 
                transform.localScale.y, 
                transform.localScale.z);
            transform.localPosition = new Vector3(
                -transform.localPosition.x, 
                transform.localPosition.y, 
                transform.localPosition.z);
        }
    }
}
