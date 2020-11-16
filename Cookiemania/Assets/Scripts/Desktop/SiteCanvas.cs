using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SiteCanvas : MonoBehaviour
{
    [SerializeField]
    [Tooltip("should have all the parts that blink for notification, with a TMP_Text " +
        "object in it's children")]
    private GameObject emailNotificationObject;
    private TMP_Text emailNotificationText;

    public static SiteCanvas Instance { get; protected set; }
    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        emailNotificationText = emailNotificationObject.
            GetComponentInChildren<TMP_Text>();
        SetEmailCount(0);
    }

    public void SetEmailCount(int count)
    {
        var toSet = count.ToString();
        if (count > 99)
        {
            toSet = "99+";
        }
        emailNotificationText.text = toSet;
        emailNotificationObject.SetActive(count > 0);
    }
}
