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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Remove()
    {
        StartCoroutine(FlashThenKill());
    }

    IEnumerator FlashThenKill()
    {
        Renderer rend = GetComponent<Renderer>();
        int timer = (int)(timeToRemove / flashPeriod);
        for (int i = 0; i < 5; i++)
        {
            rend.material.color = Color.gray;
            yield return new WaitForSeconds(timeToRemove / 5);
            rend.material.color = Color.white;
            yield return new WaitForSeconds(timeToRemove / 5);
        }
        //this could be changed to recycling the object
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
    }
}
