
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Email_Utilities;

public class EmailPreviewController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text label = null;
    [SerializeField]
    private TMP_Text sender = null;
    [SerializeField]
    private TMP_Text subject = null;
    [SerializeField]
    private GameObject readObject = null;

    private UnityAction<EventInfo> action;
    private EventInfo eventInfo;

    /// <summary>
    /// takes the entire emailinfo to store and displays the preview
    /// then runs the given action on click (presumably to swap the 
    /// email preview with the full view of this email)
    /// </summary>
    /// <param name="action"></param>
    public void Initialize(EventInfo eventInfo, 
        UnityAction<EventInfo> action, 
        bool unread = true)
    {
        this.action = action;
        this.eventInfo = eventInfo;
        SetType(label, eventInfo.Email);
        SetSubject(subject, eventInfo.Email);
        SetSenderName(sender, eventInfo.Email);
        if (unread)
        {
            GetComponent<Image>().color = Color.white;
            readObject.SetActive(false);
        }
        else
        {
            GetComponent<Image>().color = new Color(
                220f / 255f, 220f / 255f, 220f / 255f);
            readObject.SetActive(true);
        }
    }

    public void OnClick()
    {
        action?.Invoke(eventInfo);
    }
}
