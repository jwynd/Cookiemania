using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using static Email_Utilities;

// controls a single email's display
// not to be confused with the EmailPreviewController
// which shows a preview of an email and on click passes
// its info to the only email controller (there is one
// email controller and emailpreviewcontroller holds many
// previews)

// different types of emails: 
// email, email_tutorial, email_history

// need to add trigger_delay base keyword to delay when the event
// displays -> need a delayed queue for event controller that waits
// for starting a minigame or going to a tab

public class EmailController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text subject = null;
    [SerializeField]
    private TMP_Text body = null;
    [SerializeField]
    private TMP_Text sender = null;

    private EmailInfo email;

    // prefab objects necessary to display an email
    public void Initialize(EmailInfo emailInfo)
    {
        email = emailInfo;
        SetSenderName(sender, email);
        SetSubject(subject, email);
        SetBody(body, email);
    }
}
