using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperPlatformController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float timeToRemove = 1.5f;
    [SerializeField]
    private float flashPeriod = 0.25f;
    [SerializeField]
    private bool isStartingPlatform = false;

    private bool notFlashing = true;
    private Renderer rend = null;
    private Collider2D coll = null;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
        rend = GetComponent<Renderer>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Remove()
    {
        if (!isStartingPlatform && notFlashing)
        {
            StartCoroutine(FlashThenKill());
        }
        else
        {
            isStartingPlatform = false;
        }
    }

    IEnumerator FlashThenKill()
    {
        notFlashing = false;
        int timer = (int)(timeToRemove / flashPeriod);
        for (int i = 0; i < 5; i++)
        {
            rend.material.color = Color.gray;
            yield return new WaitForSeconds(flashPeriod);
            rend.material.color = Color.white;
            yield return new WaitForSeconds(flashPeriod);
        }
        //this could be changed to recycling the object
        rend.enabled = false;
        coll.enabled = false;
    }
}
