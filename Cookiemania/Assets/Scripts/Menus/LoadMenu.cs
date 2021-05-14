using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlayerDataStatics;
using static SaveSystem;

public class LoadMenu : MonoBehaviour
{
    [SerializeField]
    private bool dontDestroy = false;
    public List<GameObject> activateOnDestroy;
    public List<MonoBehaviour> enableOnDestroy;

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

    void OnDestroy()
    {
        if (enabled)
            ReactivateObjects();
    }

    private void ReactivateObjects()
    {
        foreach (GameObject g in activateOnDestroy)
        {
            g.SetActive(true);
        }
        foreach (MonoBehaviour g in enableOnDestroy)
        {
            g.enabled = true;
        }
    }

    private void OnDisable()
    {
        ReactivateObjects();
    }
}
