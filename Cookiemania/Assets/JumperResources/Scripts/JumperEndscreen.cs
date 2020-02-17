using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperEndscreen : JumperUIText
{
    [SerializeField]
    protected string constantTextPart2;
    protected override void SetText()
    {
        textRef.SetText(constantText + changingText + constantTextPart2);
    }
}
