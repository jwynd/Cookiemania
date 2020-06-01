using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterSprite : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (CharacterContentManager.Instance)
        {
            var temp = Instantiate(CharacterContentManager.Instance.characterSprite, transform);
            temp.transform.localPosition = Vector3.zero;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<Animator>().enabled = false;
            temp.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
