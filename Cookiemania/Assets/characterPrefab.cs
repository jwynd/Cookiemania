using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterPrefab : MonoBehaviour
{
    //Event stuff
    public class UnityEventConfirm : UnityEvent<string>
    {

    }

    public UnityEvent<string> CompanyUpdate = new UnityEventConfirm();

    [SerializeField]
    private SpriteRenderer _bodys;
    [SerializeField]
    private SpriteRenderer _toppings;
    [SerializeField]
    private SpriteRenderer _top;
    [SerializeField]
    private SpriteRenderer _middle;
    [SerializeField]
    private SpriteRenderer _bottom;
    [SerializeField]
    private SpriteRenderer _eyes;
    [SerializeField]
    private SpriteRenderer _eyebrows;
    private string _charName = "";
    private string _companyName = "";

    public SpriteRenderer bodys { get { return _bodys; } set { _bodys = value; } }
    public SpriteRenderer toppings { get { return _toppings; } set { _toppings = value; } }
    public SpriteRenderer top { get { return _top; } set { _top = value;  } }
    public SpriteRenderer middle { get { return _middle; } set { _middle = value;  } }
    public SpriteRenderer bottom { get { return _bottom; } set { _bottom = value;  } }
    public SpriteRenderer eyes { get { return _eyes; } set { _eyes = value;  } }
    public SpriteRenderer eyebrows { get { return _eyebrows; } set { _eyebrows = value;  } }
    public string CharName { get { return _charName; } set { _charName = value; } }
    public string CompanyName 
    { 
        get { return _companyName; } 
        set 
        { 
            _companyName = value;
            Debug.LogError("firing event"); 
            CompanyUpdate?.Invoke(_companyName);
        } 
    }
}
