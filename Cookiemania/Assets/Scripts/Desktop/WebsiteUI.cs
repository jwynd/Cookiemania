using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WebsiteUI : MonoBehaviour
{
    [SerializeField]
    protected Animator weatherAnimator = null;
    [SerializeField]
    protected TMP_Text CompanyName = null;
    [SerializeField]
    protected List<AnimationClip> animations = new List<AnimationClip>();
    protected string currentWeather = "";
    protected CharPrefab charRef = null;

    private void Awake()
    {
        currentWeather = animations[0].name;
    }

    private void Update()
    {
        if (!SaveSystem.DontLoad())
        {
            UpdateCompany(charRef.CompanyName);
        }
        enabled = false;
    }

    public void SetUpFromCharPrefab(CharPrefab charprefab)
    {
        charRef = charprefab;
        charRef.CompanyUpdate.AddListener(UpdateCompany);
    }

    public void AttachWeekListener()
    {
        PlayerData.Player.WeekChanged.AddListener(ChangeAnimation);
    }

    public void AnimateWeather()
    {
        weatherAnimator.Play(currentWeather, -1, 0f);
    }

    public void ChangeAnimation(int _arg0, int _arg1)
    {
        currentWeather = animations[Random.Range(0, animations.Count)].name;
    }

    public void UpdateCompany(string newName)
    {
        CompanyName.text = newName;
        AnimateWeather();
    }
}
