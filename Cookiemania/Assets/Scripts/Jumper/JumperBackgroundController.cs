using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperBackgroundController : MonoBehaviour
{
    [SerializeField]
    protected List<GameObject> dayPrefabs;
    [SerializeField]
    protected List<GameObject> nightPrefabs;

    protected JumperManagerGame gm;

    


    void Start()
    {
        gm = JumperManagerGame.Instance;
        SetNight(gm.Night);

    }

    public void SetNight(bool night)
    {
        foreach (var obj in dayPrefabs)
        {
            obj.SetActive(!night);
        }
        foreach (var obj in nightPrefabs)
        {
            obj.SetActive(night);
        }
        
    }


}
