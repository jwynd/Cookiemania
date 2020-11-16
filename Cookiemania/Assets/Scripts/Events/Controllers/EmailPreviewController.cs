
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

using static Email_Utilities;

public class EmailPreviewController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text label = null;
    [SerializeField]
    private TMP_Text sender = null;
    [SerializeField]
    private TMP_Text subject = null;

    private UnityAction<EmailInfo> action;
    private EmailInfo email;
    private EmailCategory type;

    /// <summary>
    /// takes the entire emailinfo to store and displays on click
    /// </summary>
    /// <param name="action"></param>
    public void Initialize(EmailCategory type, EmailInfo email, UnityAction<EmailInfo> action)
    {
        this.action = action;
        this.email = email;
        this.type = type;
        SetType();
        SetSubject();
        SetSenderName();
    }

    private void SetSenderName()
    {
        if (EventManager.Instance)
        {
            if (EventManager.Instance.CharacterDictionary.TryGetValue(
                email.senderUniqueName, out Tuple<string, Sprite> senderTuple)) 
            {
                sender.text = senderTuple.Item1;
            }
        }
        else
        {
            throw new Exception("event manager must exist for " +
                "email subsystem to access character dictionary");
        }
    }

    private void SetSubject()
    {
        subject.text = email.subject;
    }

    private void SetType()
    {
        if (EMAIL_TYPE_STRING.TryGetValue(type, out string value))
        {
            label.text = value;
        }
        else
        {
            throw new Exception("email type not supported: " + type);
        }
    }

    public void OnClick()
    {
        action?.Invoke(email);
    }
}
