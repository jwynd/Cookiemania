using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperPlatformTrigger : MonoBehaviour
{

    public JumperManagerGame gen;

    private void Start()
    {
        gen = JumperManagerGame.Instance;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) { gen.BuildSection(); }
    }
}
