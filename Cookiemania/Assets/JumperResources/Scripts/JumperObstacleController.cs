using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperObstacleController : MonoBehaviour
{
    public int damage { get; private set; }
    public bool destroyable { get; private set; }

    // Start is called before the first frame update

    void Start()
    {
        damage = 1;
        destroyable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
