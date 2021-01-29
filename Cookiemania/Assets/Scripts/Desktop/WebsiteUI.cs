using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WebsiteUI : MonoBehaviour
{
    [SerializeField]
    protected TMP_Text CompanyName = null;

    protected CharacterPrefab charRef = null;

    public void SetUpFromCharPrefab(CharacterPrefab charprefab)
    {
        charRef = charprefab;
        Debug.LogError("listening to " + charprefab.name);
        charRef.CompanyUpdate.AddListener(UpdateCompany);
    }

    public void UpdateCompany(string newName)
    {
        Debug.LogError("got " + newName);
        CompanyName.text = newName;
    }
}
