using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConfimationUI : MonoBehaviour

{
    
    [SerializeField]
    protected UnityEngine.UI.Button confirm = null;

    [SerializeField]
    protected UnityEngine.UI.Button cancel = null;

    public void SetConfirm(UnityAction func)
    {
        confirm.onClick.AddListener(func);
    }

    public void SetCancel(UnityAction func)
    {
        cancel.onClick.AddListener(func);
    }


}
