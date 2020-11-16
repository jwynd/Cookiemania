using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EmailTester : MonoBehaviour
{
    [SerializeField]
    private GameObject previewObject = null;
    [SerializeField]
    private GameObject contentHolder = null;

    private EmailPreviewController preview = null;

    private void Start()
    {
        GameObject t = Instantiate(previewObject, contentHolder.transform);
        preview = t.GetComponent<EmailPreviewController>();
        EmailInfo info = new EmailInfo("hey what's up, just checking in", 
            "Was hoping to catch you earlier buuuuut\n\nI need you to go down to the bank and make a couple transactions\n" +
            "No big deal, just get it done by tomorrow at lunch.\nBoss OUT!",
            "boss", null, null);
        preview.Initialize(Email_Utilities.EmailCategory.Starred, 
            info, 
            SwapView);
    }

    private void SwapView(EmailInfo info)
    {
        Debug.Log(info.body);
    }
}
