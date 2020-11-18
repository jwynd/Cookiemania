using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MinigameUI : MonoBehaviour
{
    [SerializeField]
    protected Button startGame1 = null;
    [SerializeField]
    protected Button startGame1alt = null;
    [SerializeField]
    protected Button startGame2 = null;
    [SerializeField]
    protected Button startGame2alt = null;
    [SerializeField]
    protected Button returnButton = null;

    public void SetGame1Listener(UnityAction fn)
    {
        UnityEventTools.AddPersistentListener(startGame1.onClick, fn);
        UnityEventTools.AddPersistentListener(startGame1alt.onClick, fn);
    }

    public void SetGame2Listener(UnityAction fn)
    {
        UnityEventTools.AddPersistentListener(startGame2.onClick, fn);
        UnityEventTools.AddPersistentListener(startGame2alt.onClick, fn);
    }
    public void SetReturnListener(UnityAction fn)
    {
        UnityEventTools.AddPersistentListener(returnButton.onClick, fn);
    }

}
