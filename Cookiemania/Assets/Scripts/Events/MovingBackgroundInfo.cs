using System;
using UnityEngine;

[Serializable]
public class MovingBackgroundInfo
{
    [SerializeField]
    private string uniqueName = "";
    // need to change this to a component that does the stage management
    // for the scene when this starts getting used
    [SerializeField]
    private GameObject backgroundObject = null;
    public string UniqueName { get { return uniqueName; } }
    public GameObject BackgroundPrefab { get { return backgroundObject; } }
}
