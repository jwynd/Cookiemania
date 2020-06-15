using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiteCanvas : MonoBehaviour
{
    public static SiteCanvas Instance { get; protected set; }
    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

}
