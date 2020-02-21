using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility_MatchActive : MonoBehaviour
{
    public GameObject match;
    // Start is called before the first frame update
    void Start()
    {
        if(match == null){
            Debug.LogError("Match is null, Destroying self");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void OnEnable()
    {
        match.SetActive(true);
    }

    void OnDisable()
    {
        match.SetActive(false);
    }
}
