﻿
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

    /// <summary>
    /// takes the entire emailinfo to store and displays the preview
    /// then runs the given action on click (presumably to swap the 
    /// email preview with the full view of this email)
    /// </summary>
    /// <param name="action"></param>
    public void Initialize(EmailInfo email, UnityAction<EmailInfo> action)
    {
        this.action = action;
        this.email = email;
        SetType(label, email);
        SetSubject(subject, email);
        SetSenderName(sender, email);
    }



    public void OnClick()
    {
        action?.Invoke(email);
    }
}
