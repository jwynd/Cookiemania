using System;
using UnityEngine;

[Serializable]
public class CharacterInfo
{
    public Sprite Sprite;
    public string DisplayName;
    [Tooltip("The unique name is case insensitive: e.g. BOSS and boss are the same")]
    public string UniqueName;

    public CharacterInfo(Sprite sprite, string displayName, string uniqueName)
    {
        this.Sprite = sprite;
        this.DisplayName = displayName;
        this.UniqueName = uniqueName.ToLowerInvariant();
    }
}