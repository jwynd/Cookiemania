using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class pierce : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody2D rigidBody;
    public Sprite EExplosionImage;
    public Transform Player;
    public GameObject user;
   // public static int moneylevel;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2f);
        Player = GameObject.FindGameObjectWithTag("treasure").GetComponent<Transform>();
        user = GameObject.FindGameObjectWithTag("Player");
        Vector3 direction = transform.position - Player.position;
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = direction * speed;
    }

    void OnBecomeInvisible()
    {
        Destroy(gameObject);
    }

    public void increaseTextUIScore()
    {
        var textUIComp = GameObject.Find("Score").GetComponent<Text>();
        int score = int.Parse(textUIComp.text);

        if (PlayerData.Player.incomelvl == 1)
        {
            score += 3;
            winmessage.coins = score;
            losemessage.Lcoins = score / 2;
            if (score % 10 == 0)
            {
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.gainlife);
                user.gameObject.GetComponent<health>().gainlife();
            }
        }
        else if (PlayerData.Player.incomelvl == 2)
        {
            score += 6;
            winmessage.coins = score;
            losemessage.Lcoins = score / 2;
            if (score % 10 == 0)
            {
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.gainlife);
                user.gameObject.GetComponent<health>().gainlife();
            }
        }
        else if (PlayerData.Player.incomelvl == 3)
        {
            score += 10;
            winmessage.coins = score;
            losemessage.Lcoins = score / 2;
            if (score % 100 == 0)
            {
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.gainlife);
                user.gameObject.GetComponent<health>().gainlife();
            }
        }
        else
        {
            score += 1;
            winmessage.coins = score;
            losemessage.Lcoins = score / 2;
        }

        textUIComp.text = score.ToString();
    }
}
