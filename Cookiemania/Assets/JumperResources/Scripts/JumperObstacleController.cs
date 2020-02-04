using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperObstacleController : MonoBehaviour
{
    [SerializeField]
    private float damage = 2.0f;
    //all obstacles can be destroyed
    
    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetDamage()
    {
        return damage;
    }
}
