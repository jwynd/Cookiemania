using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharPrefab : MonoBehaviour
{
    //Event stuff
    public class UnityEventConfirm : UnityEvent<string>
    {

    }

    public UnityEvent<string> CompanyUpdate = new UnityEventConfirm();

    [SerializeField]
    private SpriteRenderer _body = null;
    [SerializeField]
    private SpriteRenderer _topping = null;
    [SerializeField]
    private SpriteRenderer _top = null;
    [SerializeField]
    private SpriteRenderer _middle = null;
    [SerializeField]
    private SpriteRenderer _bottom = null;
    [SerializeField]
    private SpriteRenderer _eyes = null;
    [SerializeField]
    private SpriteRenderer _eyebrows = null;
    private string _charName = "";
    private string _companyName = "";

    public Sprite body { get { return _body.sprite; } set { _body.sprite = value; } }
    public Sprite topping { get { return _topping.sprite; } set { _topping.sprite = value; } }
    public Sprite top { get { return _top.sprite; } set { _top.sprite = value; } }
    public Sprite middle { get { return _middle.sprite; } set { _middle.sprite = value; } }
    public Sprite bottom { get { return _bottom.sprite; } set { _bottom.sprite = value; } }
    public Sprite eyes { get { return _eyes.sprite; } set { _eyes.sprite = value; } }
    public Sprite eyebrows { get { return _eyebrows.sprite; } set { _eyebrows.sprite = value; } }
    public string CharName { get { return _charName; } set { _charName = value; } }
    public string CompanyName
    {
        get { return _companyName; }
        set
        {
            _companyName = value;
            CompanyUpdate?.Invoke(_companyName);
        }
    }
}