using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperPlatformTrigger : MonoBehaviour
{

    public JumperManager gen;

    private void Start()
    {
        gen = JumperManager.Instance;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) { gen.BuildSection(); }
    }
}
