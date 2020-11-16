using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using static Email_Utilities;

// uses the static email backgrounds
public class EmailInfo
{
    public string subject = "";
    public string body = "";
    public string senderUniqueName = null;
    // starred for the basic EmailEvent type
    public EmailCategory emailType = EmailCategory.Starred;
    // attachments are images for simplicity atm
    // idk what theyd be, maybe posters or pictures
    // name, image_of_attached_document
    public List<Tuple<string, Sprite>> attachments = 
       new List<Tuple<string, Sprite>>();
    public ChoiceInfo choice = null;
    
    public EmailInfo(string subject, string body, string sender,
        EmailCategory type,
        ChoiceInfo choice = null,
        List<Tuple<string, Sprite>> attachments = null)
    {
        this.subject = subject;
        this.body = body;
        this.emailType = type;
        this.senderUniqueName = sender;
        this.choice = choice;
        if (attachments != null)
        {
            this.attachments = attachments;
        }
    }
}
