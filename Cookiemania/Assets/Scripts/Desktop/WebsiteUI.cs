using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using static General_Utilities.ListExtensions;

public class WebsiteUI : MonoBehaviour
{
    [SerializeField]
    protected Image Icon = null;
    [SerializeField]
    protected Image Picture1 = null;
    [SerializeField]
    protected Image Picture2 = null;
    [SerializeField]
    protected TextMeshProUGUI CompanyName = null;
    [SerializeField]
    protected TextMeshProUGUI Motto = null;
    [SerializeField]
    protected TextMeshProUGUI CompanyDescription = null;

    //in order name, motto, description
    public void SetStrings(IList<string> lines)
    {
        if (lines.Count < 3)
        {
            Debug.LogError("need to send at least 3 strings");
            return;
        }
        CompanyName.text = lines.PopFront();
        Motto.text = lines.PopFront();
        CompanyDescription.text = lines.PopFront();
    }

    //send list ordered --> icon, pic1, pic2
    public void SetSprites(IList<Sprite> sprites)
    {
        if (sprites.Count < 3)
        {
            Debug.LogError("need to send at least 3 strings");
            return;
        }
        Icon.sprite = sprites.PopFront();
        Picture1.sprite = sprites.PopFront();
        Picture2.sprite = sprites.PopFront();
    }

    private void OnEnable()
    {
        SetUpFromCustomization();
    }

    public void SetUpFromCustomization()
    {
        if (!CustomizationUI.Instance)
            return;
        SetStrings(CustomizationUI.Instance.GetTexts());
        SetSprites(CustomizationUI.Instance.GetSprites());
    }
}
