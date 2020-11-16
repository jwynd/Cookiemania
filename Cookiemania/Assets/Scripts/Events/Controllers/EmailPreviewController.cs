
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

    public EmailCategory EmailType 
    { 
        get 
        { 
            if (eventInfo != null && eventInfo.Email != null)
                return eventInfo.Email.emailType; 
            else return EmailCategory.Starred;
        } 
    }

    public bool Unread
    {
        get;
        private set;
    } = true;

    /// <summary>
    /// takes the entire emailinfo to store and displays the preview
    /// then runs the given action on click (presumably to swap the 
    /// email preview with the full view of this email)
    /// </summary>
    /// <param name="action"></param>
    public void Initialize(EventInfo eventInfo,
        UnityAction<EventInfo> action)
    {
        this.action = action;
        this.eventInfo = eventInfo;
        SetType(label, eventInfo.Email);
        SetSubject(subject, eventInfo.Email);
        SetSenderName(sender, eventInfo.Email);
        GetComponent<Image>().color = Color.white;
        readObject.SetActive(!Unread);
    }

    public void OnClick()
    {
        GetComponent<Image>().color = new Color(
                220f / 255f, 220f / 255f, 220f / 255f);
        Unread = false;
        readObject.SetActive(!Unread);
        action?.Invoke(eventInfo);
    }

    private void OnEnable()
    {
        readObject.SetActive(!Unread);
    }
}
