using System;
using UnityEngine;

[Serializable]
public class BackgroundInfo
{
    [SerializeField]
    private string uniqueName = "";
    [SerializeField]
    private Sprite background = null;
    public string UniqueName { get { return uniqueName; } }
    public Sprite Background { get { return background; } }
}
