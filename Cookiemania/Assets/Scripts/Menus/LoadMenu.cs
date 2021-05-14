using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlayerDataStatics;
using static SaveSystem;

public class LoadMenu : MonoBehaviour
{
    [SerializeField]
    private bool dontDestroy = false;


    public void Return()
    {
        if (dontDestroy)
        {
            enabled = false;
            gameObject.SetActive(false);
            return;
        }
        Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Return();
        }
    }
}
