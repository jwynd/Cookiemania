using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class JumperUIText : MonoBehaviour
{
    #region variables
    [SerializeField]
    protected TextMeshProUGUI textRef;
    [SerializeField]
    protected string constantText;
    [SerializeField]
    protected string changingText;
    #endregion

    #region privateOverwritables
    protected virtual void Start()
    {
        textRef = textRef.GetComponent<TextMeshProUGUI>();
    }
    //default is constant then changing text, very basic, all subject to change
    protected virtual void SetText()
    {
        textRef.SetText(constantText + changingText);
    }

    #endregion

    #region public
    public virtual void UpdateText(string newText)
    {
        changingText = newText;
        SetText();
    }
    #endregion
}
