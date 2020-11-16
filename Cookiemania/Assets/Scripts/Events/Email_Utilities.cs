using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// parsing_utilities has a ton of enum defs
using static Parsing_Utilities;

public static class Email_Utilities
{
    // inbox just holds all the other email categories 
    // and is unlisted here as an email will always belong to 
    // inbox and some other category
    public enum EmailCategory
    {
        Starred,
        History,
        Tutorial,
    }

    // inbox + above enum
    public enum EmailTabPossibilities
    {
        Starred,
        History,
        Tutorial,
        Inbox
    }

    public static readonly Dictionary<TypeKeyword, EmailCategory> CATEGORY_FROM_TYPE =
        new Dictionary<TypeKeyword, EmailCategory>
    {
        {TypeKeyword.EventEmail, EmailCategory.Starred },
        {TypeKeyword.TutorialEmail, EmailCategory.Tutorial },
        {TypeKeyword.HistoryEmail, EmailCategory.History },
    };

    public static bool IsEmail(this TypeKeyword type)
    {
        return CATEGORY_FROM_TYPE.ContainsKey(type);
    }

    public static void SetSenderName(TMP_Text toSet, EmailInfo email)
    {
        if (EventManager.Instance)
        {
            if (EventManager.Instance.CharacterDictionary.TryGetValue(
                email.senderUniqueName, out Tuple<string, Sprite> senderTuple))
            {
                toSet.text = senderTuple.Item1;
            }
        }
        else
        {
            throw new Exception("event manager must exist for " +
                "email subsystem to access character dictionary");
        }
    }

    public static void SetSubject(TMP_Text toSet, EmailInfo email)
    {
        var subject = email.subject;
        if (EventManager.Instance)
            subject = EventManager.Instance.
                GetDialogueWithOverwrites(subject);
        toSet.text = subject;
    }

    public static void SetType(TMP_Text toSet, EmailInfo email)
    {
        toSet.text = email.emailType.ToString();
    }

    public static void SetBody(TMP_Text toSet, EmailInfo email)
    {
        var body = email.body;
        if (EventManager.Instance)
            body = EventManager.Instance.
                GetDialogueWithOverwrites(body);
        toSet.text = body;
    }
}
