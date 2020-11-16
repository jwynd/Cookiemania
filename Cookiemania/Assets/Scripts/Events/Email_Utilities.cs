using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Email_Utilities
{
    public enum EmailCategory
    {
        Starred,
        History,
        Tutorial,
    }

    public static readonly Dictionary<EmailCategory, string> EMAIL_TYPE_STRING = 
        new Dictionary<EmailCategory, string>
    {
        {EmailCategory.Starred, "Starred"},
        {EmailCategory.History, "History"},
        {EmailCategory.Tutorial, "Tutorial"},
    };
}
