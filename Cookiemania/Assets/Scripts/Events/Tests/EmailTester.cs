using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using static Email_Utilities;

public class EmailTester : MonoBehaviour
{
    [SerializeField]
    private GameObject previewPrefab = null;
    [SerializeField]
    private GameObject previewsHolder = null;
    [SerializeField]
    private EmailController emailController = null;

    [Serializable]
    public class ButtonType
    {
        [SerializeField]
        private Button button = null;
        [SerializeField]
        private EmailTabPossibilities type = EmailTabPossibilities.Inbox;
        public Button Button { get { return button; } }
        public EmailTabPossibilities Type { get { return type; } }
    }

    [SerializeField]
    private List<ButtonType> categories = new List<ButtonType>();

    // unread email order can change by random email extraction
    private List<EmailInfo> unreadEmails = new List<EmailInfo>();
    // read email list order will never change and should show up in stack order
    private Stack<EmailInfo> readEmails = new Stack<EmailInfo>();
    // preview mode always set to true when tab is opened / category is clicked
    // initial category is always whatever the first tab in the categories list is
    private bool previewMode = false;
    public bool PreviewMode
    {
        get { return previewMode; }
        private set
        {
            if (previewMode != value)
            {
                previewMode = value;
                previewsHolder.SetActive(previewMode);
                emailController.gameObject.SetActive(!previewMode);
            }
        }
    }

    private void Awake()
    {
        if (categories.Count > Enum.GetNames(typeof(EmailTabPossibilities)).Length)
        {
            throw new Exception("number of categories may not exceed the number of " +
                "possible email tabs");
        }
        
    }

    public void ShowYourself()
    {

        // show that the button has been selected :)
        categories[0].Button.Select();
        categories[0].Button.onClick.Invoke();
    }

    public void SetActiveTab(int v)
    {
        // whenever tab is set, preview mode turns on
        PreviewMode = true;
        EmailTabPossibilities activeTab = categories[v].Type;
        HashSet<EmailCategory> visibleTypes = new HashSet<EmailCategory>();
        if (activeTab == EmailTabPossibilities.Inbox)
        {
            foreach (EmailCategory value in Enum.GetValues(
                typeof(EmailCategory)).Cast<EmailCategory>())
            {
                visibleTypes.Add(value);
            }
        }
        else
        {
            visibleTypes.Add((EmailCategory)
                Enum.Parse(typeof(EmailCategory), activeTab.ToString()));
        }
        Debug.Log(string.Join(", ", visibleTypes));
    }

    private void Start()
    {
        ShowYourself();
        var t = Instantiate(previewPrefab, previewsHolder.transform);
        var preview = t.GetComponent<EmailPreviewController>();
        var info = new EmailInfo("hey what's up, just checking in", 
            "Was hoping to catch you earlier buuuuut\n\nI need you to go down to the bank and make a couple transactions\n" +
            "No big deal, just get it done by tomorrow at lunch.\nBoss OUT!",
            "boss", EmailCategory.Starred, null, null);
        preview.Initialize(info, ViewEmail);
    }



    private void ViewEmail(EmailInfo info)
    {
        emailController.Initialize(info);
        PreviewMode = false;
    }
}
