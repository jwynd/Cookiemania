using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperTextAdvanced : JumperGeneralText
{
    [SerializeField]
    protected string constantTextPart2;
    protected override void SetText()
    {
        textRef.SetText(constantText + changingText + constantTextPart2);
    }
}
