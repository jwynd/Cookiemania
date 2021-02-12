using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
public class LeaderBoard : MonoBehaviour
{
    // Start is called before the first frame update
    private List<GameObject> userstatistics = new List<GameObject>();
    private int rand;
    private string hold;
    private List<string> Leaders = new List<string>();
    void Start()
    {
        Leaders.Add("Snickers");
        Leaders.Add("Doodle");
        Leaders.Add("Kat");
        Leaders.Add("Coco");
        Leaders.Add("Nilla");
        Leaders.Add("Berry");
        Leaders.Add("Dulce");
        Leaders.Add("Sugar");
        Leaders.Add("Latte");
        Leaders.Add("Milky");
        Leaders.Add("Mr. M");
        Leaders.Add("Vanilla");
        Leaders.Add("Velvet");
        Leaders.Add("Spice");
        Leaders.Add("Cacao");
        Leaders.Add("Strawberry Ice");
        Leaders.Add("Coffee Toffee");
        Leaders.Add("Sprink Les");
        Leaders.Add("P. Bark");
        Leaders.Add("Peppermint");
        Leaders.Add("Minty Fresh");
        Leaders.Add("Sweetness");
        Leaders.Add("Mint");
        Leaders.Add("Flora Flour");
        Leaders.Add("K. Kringle");
        Leaders.Add("Lil Leche");
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            userstatistics.Add(gameObject.transform.GetChild(i).gameObject);

        }

        //Temporary testing -> these functions should be called after purchasing AND updated after every few minutes or every game.
        //if certain marketing object is purchased
        Leadernames();
    }


    // Update is called once per frame
    void Update()
    {

    }
    public void Leadernames()
    {
        Leaders.Shuffle();
        for (int i = 0; i < userstatistics.Count; i++)
        {

            userstatistics[i].GetComponent<TMPro.TextMeshProUGUI>().text = Leaders[i];
        }
    }

}
