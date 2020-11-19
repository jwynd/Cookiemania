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
        startGame1.onClick.AddListener(fn);
        startGame1alt.onClick.AddListener(fn);
    }

    public void SetGame2Listener(UnityAction fn)
    {
        startGame2.onClick.AddListener(fn);
        startGame2alt.onClick.AddListener(fn);
    }
    public void SetReturnListener(UnityAction fn)
    {
        returnButton.onClick.AddListener(fn);
    }

}
