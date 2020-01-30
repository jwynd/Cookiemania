using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class playerfire : MonoBehaviour
{
    public float speed = 20;
    private Rigidbody2D rigidBody;
    public Sprite EExplosionImage;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = Vector2.up * speed;
    } 

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag == "Enemy")
        {
            //SoundManager.Instance.PlayOneShot(SoundManager.Instance.enemydeath);
            increaseTextUIScore();

            col.GetComponent<SpriteRenderer>().sprite = EExplosionImage;
            Destroy(gameObject);
            DestroyObject(col.gameObject, 0.5f); //DestroyObject(col.gameObject, 0.5f)

        }

        if (col.tag == "shield") 
        {
            Destroy (gameObject);
            DestroyObject(col.gameObject);
        }
        
    }
    void OnBecomeInvisible()
    {
        Destroy(gameObject);
    }

    void increaseTextUIScore()
    {
        var textUIComp = GameObject.Find("Score").GetComponent<Text>();
        int score = int.Parse(textUIComp.text);

        score += 10;

        textUIComp.text = score.ToString();
    }
}
