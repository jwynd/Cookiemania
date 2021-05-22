using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class shield : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    public Transform Player;
    public Sprite EExplosionImage;
    public static int shieldhitlvl = 0;
    int hit;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerData.Player != null)
        {
            shieldhitlvl = PlayerData.Player.ShieldHealth;
        }
        Player = GameObject.FindGameObjectWithTag("treasure").GetComponent<Transform>();
        Vector3 direction = transform.position - Player.position;
        transform.Translate(direction * 1.5f);
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = direction;
        hit = 0;

    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.CompareTag("Enemy"))
        {
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemydies);
            increaseTextUIScore();

            col.gameObject.GetComponent<SpriteRenderer>().sprite = EExplosionImage;
            Destroy(col.gameObject, 0.2f); //DestroyObject(col.gameObject, 0.5f)
            hit += 1;
            Debug.Log("enemy hit");
            if (shieldhitlvl == 0)
            {
                Destroy(gameObject);
                Debug.Log("enemy hit destroy 0");
            } else if (shieldhitlvl == 1 && hit >= 2)
            {
                Destroy(gameObject);
                Debug.Log("enemy hit destroy 1");
            } else if (shieldhitlvl == 2 && hit >= 3)
            {
                Destroy(gameObject);
                Debug.Log("enemy hit destroy 2");
            }
        }

        /*if (col.gameObject.CompareTag("fire"))
        {
            Destroy(gameObject);
            Destroy(col.gameObject);
        }*/
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("enemyfire"))
        {
            Destroy(col.gameObject);
            hit += 1;
            Debug.Log("fire hit");
            if (shieldhitlvl == 0)
            {
                Destroy(gameObject);
            }
            else if (shieldhitlvl == 1 && hit >= 2)
            {
                Destroy(gameObject);
                Debug.Log("hit lvl 1");
            }
            else if (shieldhitlvl == 2 && hit >= 3)
            {
                Destroy(gameObject);
                Debug.Log("hit lvl 2");
            }
        }
    }
    
    void OnBecomeInvisible()
    {
        Destroy(gameObject);
    }

    public void increaseTextUIScore()
    {
        var textUIComp = GameObject.Find("Score").GetComponent<Text>();
        int score = int.Parse(textUIComp.text);
        if(PlayerData.Player == null)
        {
            score += 3;
            winmessage.coins = score;
            losemessage.Lcoins = score / 2;
            Debug.Log("this is a remote instance of space mini");
        } else { 
        if (PlayerData.Player.incomelvl == 1)
        {
            score += 3;
            winmessage.coins = score;
            losemessage.Lcoins = score / 2;
        }
        else if (PlayerData.Player.incomelvl == 2)
        {
            score += 6;
            winmessage.coins = score;
            losemessage.Lcoins = score / 2;
        }
        else if (PlayerData.Player.incomelvl == 3)
        {
            score += 10;
            winmessage.coins = score;
            losemessage.Lcoins = score / 2;
        }
        else
        {
            score += 1;
            winmessage.coins = score;
            losemessage.Lcoins = score / 2;
        }
        }
        textUIComp.text = score.ToString();
    }
    
    private void Update()
    {
        if (shieldhitlvl == 0)
        {
            Destroy(this.gameObject, 5f);
        } else if(shieldhitlvl == 1)
        {
            Destroy(this.gameObject, 8f);
        } else if(shieldhitlvl == 2)
        {
            Destroy(this.gameObject, 12f);
        }
    }
}