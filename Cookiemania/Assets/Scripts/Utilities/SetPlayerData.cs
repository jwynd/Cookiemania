﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using static PlayerDataStatics;

public class SetPlayerData : MonoBehaviour
{
    public PlayerDataProperty PropertyName;
    public TMPro.TextMeshProUGUI valueText;
    public Button decrementButton;

    private PlayerData data;
    private int incAmount = 1;

    // Start is called before the first frame update
    void Start()
    {
        data = PlayerData.Player;
        GetComponent<TMPro.TextMeshProUGUI>().text = PropertyName.ToString();
        if (PropertyName == PlayerDataProperty.money)
            incAmount = 100;
        else if (PropertyName == PlayerDataProperty.week)
            decrementButton.interactable = false;
        StartCoroutine(initValue());
    }

    private IEnumerator initValue()
    {
        var wait = new WaitForEndOfFrame();
        yield return wait;
        yield return wait;
        yield return wait;
        yield return wait;
        yield return wait;
        valueText.text = data.TestSetter(PropertyName, 0).ToString();
    }

    public void Increment()
    {
        valueText.text = data.TestSetter(PropertyName, incAmount).ToString();
    }

    public void Decrement()
    {
        valueText.text = data.TestSetter(PropertyName, -incAmount).ToString();
    }

}
