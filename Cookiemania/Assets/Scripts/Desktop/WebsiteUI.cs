using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WebsiteUI : MonoBehaviour
{
    [SerializeField]
    protected Animator weatherAnimator = null;
    [SerializeField]
    protected TMP_Text CompanyName = null;

    protected CharacterPrefab charRef = null;

    public void SetUpFromCharPrefab(CharacterPrefab charprefab)
    {
        charRef = charprefab;
        charRef.CompanyUpdate.AddListener(UpdateCompany);
    }

    public void AnimateWeather()
    {
        weatherAnimator.Play("sunny", -1, 0f);
    }

    public void UpdateCompany(string newName)
    {
        CompanyName.text = newName;
        AnimateWeather();
    }
}
