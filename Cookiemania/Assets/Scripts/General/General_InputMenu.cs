using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General_InputMenu : General_Input
{
    [SerializeField]
    protected List<KeyCode> openMenuKeys;
    [SerializeField]
    protected List<KeyCode> resumeKeys;
    [SerializeField]
    protected List<KeyCode> settingsKeys;
    [SerializeField]
    protected List<KeyCode> exitKeys;
    [SerializeField]
    protected List<KeyCode> enterKeys;
    [SerializeField]
    protected List<KeyCode> leftKeys;
    [SerializeField]
    protected List<KeyCode> rightKeys;
    [SerializeField]
    protected List<KeyCode> upKeys;
    [SerializeField]
    protected List<KeyCode> downKeys;

    #region public
    public float Resume { get { return ResumeInput(); } }
    public float Horizontal { get { return HorizontalInput(); } }
    public float Vertical { get { return VerticalInput(); } }
    public float Settings { get { return SettingsInput(); } }
    public float Exit { get { return ExitInput(); } }
    public float Enter { get { return EnterInput(); } }
    public float OpenMenu { get { return OpenMenuInput(); } }

    #endregion

    #region input handling
    protected float ResumeInput()
    {
        return General_Input.GetKeyDown(resumeKeys);
    }
    protected float HorizontalInput()
    {
        return General_Input.GetKeyDown(rightKeys, leftKeys);
    }

    protected float VerticalInput()
    {
        return General_Input.GetKeyDown(upKeys, downKeys);
    }
    protected float SettingsInput()
    {
        return General_Input.GetKeyDown(settingsKeys);
    }
    protected float ExitInput()
    {
        return General_Input.GetKeyDown(exitKeys);
    }
    protected float EnterInput()
    {
        return General_Input.GetKeyDown(enterKeys);
    }

    protected float OpenMenuInput()
    {
        return General_Input.GetKeyDown(openMenuKeys);
    }

    #endregion

    #region overrides

    protected override void SetAxesDelegates()
    {
        for (int i = 0; i < Axes.Count; i++)
        {
            var axis = Axes[i];
            axisReturnDelegates.Add(null);
            switch (axis)
            {
                case nameof(Horizontal):
                    axisReturnDelegates[i] = HorizontalInput;
                    break;
                case nameof(Resume):
                    axisReturnDelegates[i] = ResumeInput;
                    break;
                case nameof(Vertical):
                    axisReturnDelegates[i] = VerticalInput;
                    break;
                case nameof(Settings):
                    axisReturnDelegates[i] = SettingsInput;
                    break;
                case nameof(Exit):
                    axisReturnDelegates[i] = ExitInput;
                    break;
                case nameof(Enter):
                    axisReturnDelegates[i] = EnterInput;
                    break;
                case nameof(OpenMenu):
                    axisReturnDelegates[i] = OpenMenuInput;
                    break;
                default:
                    Debug.LogWarning("Unsupported axis for jumper input");
                    break;
            }
        }
    }

    #endregion
}
