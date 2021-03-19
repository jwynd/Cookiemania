using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class linegraph : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private List<Sprite> line = new List<Sprite>();
    Sprite chosen;
    [SerializeField] private Image currentline;
   
    void Start()
    {
        Randomline();
    }


    // Update is called once per frame
    void Update()
    {

    }
    public void Randomline()
    {
        line.Shuffle();
        chosen = line[0];
        currentline.sprite = chosen;
        
    }
}
