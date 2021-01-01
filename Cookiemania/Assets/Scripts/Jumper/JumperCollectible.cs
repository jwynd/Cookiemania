using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperCollectible : JumperGeneralPickup
{
    protected int offsetValue = 3;
    protected override void Start()
    {
        base.Start();
        //change this to x when we switch sprites lol
        JumperGeneralPlatform dad = transform.parent.GetComponent<JumperGeneralPlatform>();
        Vector3 pBounds = dad.GetHorizontalBounds();

        //get info from parent, set parent child to this, then 
        var parentLeft = pBounds.x;
        var parentRight = pBounds.z;
        transform.position = new Vector2(Random.Range(parentLeft - offsetValue, parentRight + offsetValue), 
            Random.Range(transform.position.y, transform.position.y + offsetValue));
    }
}
