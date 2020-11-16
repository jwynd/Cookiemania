using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using static Email_Utilities;

public class EmailController : MonoBehaviour
{
    [SerializeField]
    private GameObject previewPrefab = null;
    [SerializeField]
    private GameObject previewsHolder = null;
    [SerializeField]
    private EmailViewController emailController = null;

    public delegate void OnComplete(EventInfo info, bool delayCallback);

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
    // read email list order will never change and should show up in stack order
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
        foreach (Transform child in previewsHolder.transform)
        {
            child.gameObject.SetActive(true);
        }
        var list = previewsHolder.GetComponentsInChildren<EmailPreviewController>();
        foreach (var comp in list)
        {
            comp.gameObject.SetActive(visibleTypes.Contains(comp.EmailType));
        }
        Debug.Log(string.Join(", ", visibleTypes));
    }

    private void Start()
    {
        // only need to instantiate when adding an email
        //TestFunction();
    }

    private void TestFunction()
    {
        ShowYourself();
        var t = Instantiate(previewPrefab, previewsHolder.transform);
        var preview = t.GetComponent<EmailPreviewController>();
        var eventInfo = new EventInfo("blarg");
        eventInfo.Email = new EmailInfo("hey what's up, just checking in",
            "Was hoping to catch you earlier buuuuut\n\nI need you to go down to the bank and make a couple transactions\n" +
            "No big deal, just get it done by tomorrow at lunch.\nBoss OUT!",
            "boss", EmailCategory.Starred, null, null);
        preview.Initialize(eventInfo, ViewEmail);
    }

    public void AddEmail(EventInfo eventInfo)
    {
        if (!eventInfo.EventType.IsEmail() || eventInfo.Email == null)
        {
            throw new Exception("cannot add an event of wrong type as email" +
                " and event must have an email");
        }
        // add it to previewsholder in first position
        var t = Instantiate(previewPrefab, previewsHolder.transform);
        t.transform.SetAsFirstSibling();
        var preview = t.GetComponent<EmailPreviewController>();
        preview.Initialize(eventInfo, ViewEmail);
        SetReadNotifications();
    }

    private void SetReadNotifications()
    {
        if (!SiteCanvas.Instance)
            return;
        var total = previewsHolder.
            GetComponentsInChildren<EmailPreviewController>();
        var unreadCount = 0;
        foreach (var comp in total)
        {
            if (comp.Unread)
                unreadCount++;
        }
        SiteCanvas.Instance.SetEmailCount(unreadCount);
    }

    private void ViewEmail(EventInfo info)
    {
        // read / unread information stored local to email preview controller
        emailController.Initialize(info);
        SetReadNotifications();
        PreviewMode = false;
    }
}
