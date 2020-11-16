using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using static Email_Utilities;
using static Parsing_Utilities;

// controls a single email's display
// not to be confused with the EmailPreviewController
// which shows a preview of an email and on click passes
// its info to the only email controller (there is one
// email controller and emailpreviewcontroller holds many
// previews)

public class EmailViewController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text subject = null;
    [SerializeField]
    private TMP_Text body = null;
    [SerializeField]
    private TMP_Text sender = null;
    [SerializeField]
    private Button choiceButton = null;

    private EmailInfo email;
    private EventInfo eventInfo;
    private ChoiceController.OnComplete hijackedAction;
    private bool choiceMadeAlready = false;
    private bool readBefore = false;

    private void Awake()
    {
        hijackedAction = HijackedChoiceAction;
    }

    // prefab objects necessary to display an email
    public void Initialize(EventInfo eventInfo)
    {
        this.eventInfo = eventInfo;
        email = eventInfo.Email;
        SetSenderName(sender, email);
        SetSubject(subject, email);
        SetBody(body, email);
        // need buttons at the bottom of the email for choices
        // player can make
        // need to return the info back up to the event controller
        var haveChoice = email.choice != null && !choiceMadeAlready;
        choiceButton.gameObject.SetActive(haveChoice);
        if (!haveChoice && !readBefore)
        {
            email.emailComplete.Invoke(eventInfo, true);
        }
        readBefore = true;
    }

    public void HijackedChoiceAction(string nextBranch,
        string choicePrompt,
        string choiceMade,
        List<Tuple<RewardKeyword, int>> rewardList,
        TypeKeyword type)
    {
        eventInfo.Email.choiceAction.Invoke(nextBranch, choicePrompt, choiceMade,
            rewardList, type);
        eventInfo.Email.emailComplete.Invoke(eventInfo, false);
    }

    public void OnReplyToChoice()
    {
        if (!EventManager.Instance)
        {
            return;
        }
        choiceMadeAlready = true;
        EventManager.Instance.ChoicePrefab.GetComponent<ChoiceController>().
            Initialize(eventInfo.Email.choice,
            eventInfo.EventType, hijackedAction);
        choiceButton.gameObject.SetActive(false);
    }
}
