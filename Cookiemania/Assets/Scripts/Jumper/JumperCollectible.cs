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
        dad.tertiaryChildren.Add(this);
        transform.parent = null;
    }
}
