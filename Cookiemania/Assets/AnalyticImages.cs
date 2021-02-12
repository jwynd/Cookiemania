using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticImages : MonoBehaviour
{
    // Start is called before the first frame update
    private List<GameObject> userstatistics = new List<GameObject>();
    private int rand;
    private string hold;
    void Start()
    {
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            userstatistics.Add(gameObject.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerData.Player.userstats == 1)
        {
            userinfo();
        }
    }

    void userinfo()
    {
        if (PlayerData.Player.userstats == 1)
        {
            for (int i = 0; i < userstatistics.Count; i++)
            {
                rand = Random.Range(1, 10000);
                hold = rand.ToString();
                userstatistics[i].GetComponent<TMPro.TextMeshProUGUI>().text = userstatistics[i].GetComponent<TMPro.TextMeshProUGUI>().text + hold;
            }
            rand = Random.Range(18, 70);
            hold = rand.ToString();
            userstatistics[2].GetComponent<TMPro.TextMeshProUGUI>().text = "Average Age of User: " + hold;
            rand = Random.Range(1, 10);
            hold = rand.ToString();
            userstatistics[1].GetComponent<TMPro.TextMeshProUGUI>().text = "Average stay of User: " + hold + " hrs";
            PlayerData.Player.userstats += 1;
        }
    }

    void LeaderBoard()
    {
        //todo
    }

    void piechart()
    {
        //todo
    }

    void linegraph()
    {
        //todo need varying lines first
    }
}
