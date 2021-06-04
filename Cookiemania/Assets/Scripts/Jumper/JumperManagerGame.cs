﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static General_Utilities.AnimationCurveLerp;

public class JumperManagerGame : MonoBehaviour
{

    #region variables
    [Serializable]
    public class Platforms
    {
        [Tooltip("Spawnable object prefabs")]
        public GameObject platform = null;
        [Tooltip("Weighted possibility to spawn (relative to other objects weights.) Prefabs in the middle of " +
            "the array are more likely due to using pseudo normal distribution.")]
        [Range(0.01f, 1.0f)]
        public float weight = 0.1f;
        public bool dangerous = false;
        public float probability { get; set; } = 0f;
    }

    [SerializeField]
    private List<Platforms> nightPrefabs = new List<Platforms>();
    [SerializeField]
    private List<Platforms> dayPrefabs = new List<Platforms>();
    [SerializeField]
    private GameObject coinPrefab = null;
    [SerializeField]
    private GameObject nightExit = null;
    [SerializeField]
    private GameObject dayExit = null;
    [SerializeField]
    private JumperSemiPermanentPlatforms startingPlatform = null;
    [SerializeField]
    private Sprite defaultDayPlatformImage = null;
    
    [Tooltip("Base height to finish the level")]
    public float heightGoal = 100f;
    [Tooltip("Maximum height player can fall")]
    public float maxFallDistance = 15f;
    public float alteredGravity = 2f;
    public float bounceHeightMultiplier = 3f;
    [SerializeField]
    [Tooltip("Important game tag, ensure this is added to the tags list")]
    protected string playerTag = "Player";
    public string PlayerTag { get { return playerTag; } }
    [SerializeField]
    [Tooltip("Important game tag, ensure this is added to the tags list")]
    protected string enemyTag = "Enemy";
    public string EnemyTag { get { return playerTag; } }
    [SerializeField]
    [Tooltip("Important game tag, ensure this is added to the tags list")]
    protected string obstacleTag = "Obstacle";
    public string ObstacleTag { get { return obstacleTag; } }
    [SerializeField]
    [Tooltip("Important game tag, ensure this is added to the tags list")]
    protected string groundTag = "Platform";
    public string GroundTag { get { return groundTag; } }
    [SerializeField]
    [Tooltip("Important game tag, ensure this is added to the tags list")]
    protected string collectiblesTag = "Pickup";
    public string CollectiblesTag { get { return collectiblesTag; } }
    public bool Night { get; protected set; } = false;
    [Range(1f, 45f)]
    [Tooltip("Starting difficulty of the level")]
    public float startingDifficulty = 1.0f;
    [Tooltip("How much difficulty increases every time a region is built")]
    public float difficultyIncrement = 0.3f;


    //game manager always tagged with game controller
    protected const string myTag = "GameController";
    private const float FLASH_COUNT = 12f;
    private const float MIN_FLASH_INTERVAL = 0.01f;
    protected float minHeightIncrease = 0.0f;

    protected float width = 10.0f;
    protected int density = 15;
    protected float offset;
    protected float height;
    protected float maxHeightReached;
    protected bool canPlaceAboveLast = true;
    protected float firstPrefabWidth = 0f;
    protected bool haveBuiltExit = false;
    protected int max;
    protected float weightRange = 0.0f;
    protected float checkAgainstPosition;
    protected float jumpHeight;
    protected List<Platforms> actualPrefabs;
    protected GameObject actualExit;
    protected bool TestingMode = false;
    protected float minDifficulty = 0.3f;
    private bool building;

    public JumperCameraController MainCam { get; private set; }
    public JumperPlayerController Player { get; private set; }

    public AnimationCurve deathFlashCurve;
    public static JumperManagerGame Instance { get; private set; }
    public float StartingDifficulty { get { return startingDifficulty; } }

    public int CoinJump { get; private set; } = 0;
    public int Health { get; private set; } = 0;
    public int JumpPower { get; private set; } = 0;
    public int MagnetAvailable { get; private set; } = 1;
    public int MagnetCD { get; private set; } = 0;
    public int MagnetRange { get; private set; } = 0;
    public int Shield { get; private set; } = 0;
    public int AI { get; private set; } = 3;

    // copied from player data 

    #endregion

    #region startup

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        InitFromPlayerData();
        RunAwakeInit();
    }

    private void InitFromPlayerData()
    {
        if (!PlayerData.Player)
        {
            Debug.LogError("Player data must exist before instantiation of this game");
            return;
        }
        var moralityMult = 0f;
        var badGuy = PlayerData.Player.morality < 0;
        Night = badGuy;
        if (badGuy)
        {
            moralityMult += -1f / PlayerData.Player.morality;
        }
        PlayerData.Player.JTimesPlayed++;
        CoinJump = PlayerData.Player.JCoinJump;
        MagnetAvailable = PlayerData.Player.JMagnet;
        MagnetCD = PlayerData.Player.JMagnetCD;
        MagnetRange = PlayerData.Player.JMagnetDistance;
        Shield = PlayerData.Player.JShield;
        JumpPower = PlayerData.Player.JJumpPower;
        AI = PlayerData.Player.ai;
        Health = PlayerData.Player.healthlvl;
        startingDifficulty = ( PlayerData.Player.difficulty - moralityMult ) / 1.4f;
        minDifficulty = Mathf.Max(startingDifficulty, 0.3f);
    }

    private void RunAwakeInit()
    {
        PrefabInit();
        Player = FindObjectOfType<JumperPlayerController>().gameObject.GetComponent<JumperPlayerController>();
        MainCam = FindObjectOfType<JumperCameraController>().gameObject.GetComponent<JumperCameraController>();
        heightGoal *= startingDifficulty;
    }

    private void PrefabInit()
    {
        weightRange = 0;
        if (Night)
        {
            actualPrefabs = nightPrefabs;
            actualExit = nightExit;
        }
        else
        {
            actualPrefabs = dayPrefabs;
            actualExit = dayExit;
            startingPlatform.gameObject.GetComponent<SpriteRenderer>().sprite = defaultDayPlatformImage;
        }
        foreach (var fab in actualPrefabs)
        {
            if (fab.dangerous)
                fab.weight *= minDifficulty;
            fab.probability = fab.weight + weightRange;
            weightRange += fab.weight;
        }
    }

    void OnValidate() 
    {
        if (transform.childCount != 0)
        {
            Debug.LogError("Ensure Spawner has no children");
        }
    }

    private void ValidationChecks()
    {
        if (!Player || !MainCam || actualPrefabs.Count == 0)
        {
            Debug.LogError("need a player, platform prefabs and camera with appropriate scripts" +
                " instantiated in the scene");
        }
        if (actualPrefabs[0].platform == null)
        {
            Debug.LogError("Platforms should not be null references");
        }
        gameObject.tag = myTag;
        TagExists(PlayerTag);
        TagExists(GroundTag);
        TagExists(EnemyTag);
        TagExists(ObstacleTag);
        TagExists(CollectiblesTag);
        gameObject.tag = myTag;
    }

    private void TagExists(string tag)
    {
        gameObject.tag = tag;
        if (gameObject.CompareTag(myTag))
        {
            Debug.LogError("All tags submitted to game manager must exist");
        }
    }

    void Start()
    {
        ValidationChecks();
        GameObject g = Instantiate(actualPrefabs[0].platform, transform.position, Quaternion.identity);
        firstPrefabWidth = g.GetComponent<Renderer>().bounds.size.x;
        minHeightIncrease = Player.gameObject.GetComponent<Renderer>().bounds.size.y * 2f + g.GetComponent<Renderer>().bounds.size.y;
        Destroy(g);
        jumpHeight = ( Player.GetOriginalJumpStrength() * alteredGravity ) / 3;
        width = Player.GetMaxVelocity() * 2.2f;
        max  = (int)(density * 1.5f);
        offset = Player.transform.position.x;
        height = Player.transform.position.y;
        maxHeightReached = Player.transform.position.y;
        BuildSection();
    }

    private GameObject Roulette()
    {
        //the 0 possibility is caught on startup
        if (actualPrefabs.Count < 2) { return actualPrefabs[0].platform; }
        float random = UnityEngine.Random.Range(0, weightRange/2) + UnityEngine.Random.Range(0, weightRange/2);
        foreach (var fab in actualPrefabs)
        {
            if (random < fab.probability)
            {
                return fab.platform;
            }
        }
        return actualPrefabs[actualPrefabs.Count - 1].platform;
    }

    private void PlaceCoins(Transform platform)
    {
        // 75% no coins
        if (UnityEngine.Random.Range(0f, 1f) > 0.75f) return;
        var modpos = platform.position;
        modpos.y += 2f;
        int amt = UnityEngine.Random.Range(1, 3);
        for (int i = 0; i < amt; i++)
        {
            var go = Instantiate(coinPrefab);
            go.transform.position = modpos + (UnityEngine.Random.rotation * Vector3.one * 1.2f);
            modpos = go.transform.position;
            go.transform.parent = platform;
        }
    }

    #endregion

    #region update
    private void FixedUpdate()
    {
        if (Player == null) { return; }
        float heightRightNow = Player.transform.position.y;
        maxHeightReached = heightRightNow > maxHeightReached ? heightRightNow : maxHeightReached;
        if (maxHeightReached - maxFallDistance >= heightRightNow)
        {
            JumperManagerUI.Instance.End(false, false);
        }
        if (!building && heightRightNow > checkAgainstPosition)
        {
            StartCoroutine(BuildWrapper());
        }
    }

    private IEnumerator BuildWrapper()
    {
        building = true;
        yield return new WaitForEndOfFrame();
        BuildSection();
        building = !haveBuiltExit;
    }

    private void AttemptToBuildExit()
    {
        if (haveBuiltExit) { return; }
        float randomx = BuildHelper();
        float randomy = UnityEngine.Random.Range(height + minHeightIncrease, height + jumpHeight);
        Vector3 pos = new Vector3(randomx, randomy, 0);
        var obj = Instantiate(actualExit, pos, Quaternion.Euler(0, 0, 0));
        obj.transform.parent = transform;
        haveBuiltExit = true;
    }

    private float BuildHelper()
    {
        float ran;
        if (canPlaceAboveLast)
        {
            ran = UnityEngine.Random.Range(offset - width, offset + width);
        }
        else
        {
            ran = UnityEngine.Random.Range(0f, 1f);
            if (ran > 0.5f)
            {
                ran = UnityEngine.Random.Range(offset + firstPrefabWidth, offset + width);
            }
            else
            {
                ran = UnityEngine.Random.Range(offset - width, offset - firstPrefabWidth);
            }
        }
        return ran;
    }

    #endregion

    #region publicFunctions

    public void BuildSection()
    {
        if (haveBuiltExit) { return; }
        startingDifficulty += difficultyIncrement;
        for (int i = 0; i < density; ++i)
        {
            float randomy = canPlaceAboveLast ? UnityEngine.Random.Range(height + minHeightIncrease, height + jumpHeight) :
                UnityEngine.Random.Range(height + (minHeightIncrease * bounceHeightMultiplier), 
                height + (jumpHeight * bounceHeightMultiplier));
            if (randomy >= heightGoal)
            {
                AttemptToBuildExit();
                break;
            }
            float randomx = BuildHelper();
            Vector3 pos = new Vector3(randomx, randomy, 0);
            offset = pos.x;
            height = pos.y;
            GameObject g = Instantiate(Roulette(), pos, Quaternion.Euler(0, 0, 0));
            PlaceCoins(g.transform);
            canPlaceAboveLast = g.GetComponent<JumperGeneralPlatform>().CanPlacePlatformsAbove();
            g.transform.parent = transform;
            g.transform.SetAsFirstSibling();
            if (i == density - 3)
            {
                checkAgainstPosition = pos.y;
            }
            if(transform.childCount > max) 
                transform.GetChild(transform.childCount-1).gameObject.GetComponent<JumperGeneralPlatform>().Remove();
        }
    }

    public float GetDifficultyMultiplier()
    {
        return startingDifficulty;
    }

    public float GetHeightGoal()
    {
        return heightGoal;
    }

    public float GetMaxHeightReached()
    {
        return maxHeightReached;
    }

    public string GetPlayerTag()
    {
        return PlayerTag; 
    }

    public string GetEnemyTag()
    {
        return EnemyTag;
    }

    public string GetObstacleTag()
    {
        return ObstacleTag;
    }

    public string GetGroundTag()
    {
        return GroundTag;
    }

    public string GetCollectiblesTag()
    {
        return CollectiblesTag;
    }

    public float GetAlteredGravity()
    {
        return alteredGravity;
    }

    //credit: https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
    //maps from old range to new range
    //not safe if oldmin and oldmax are the same values
    public static float Map(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        float map = value - oldMin;
        map /= oldMax - oldMin;
        map *= newMax - newMin;
        map += newMin;
        return map;
    }

    public static IEnumerator FlashThenKill(GameObject selfReference, float totalTime, 
        float flashInterval, JumperGeneralThreat killableChild = null)
    {
        if (killableChild != null) { killableChild.PlatformDestroyed(totalTime, flashInterval); }
        Renderer rend = selfReference.GetComponent<Renderer>();
        var incrementTimeBy = totalTime * (1f / FLASH_COUNT);
        var time = 0f;
        var i = 0f;
        while (i < totalTime && time < totalTime)
        {
            if (!rend)
                break;
            rend.material.color = rend.material.color != Color.gray ? Color.gray : Color.white;
            time += incrementTimeBy;
            var nextValue = Interpolate(Instance.deathFlashCurve, 0f, totalTime, time / totalTime);
            if (nextValue - i < MIN_FLASH_INTERVAL)
            {
                nextValue = i + MIN_FLASH_INTERVAL;
            }
            yield return new WaitForSeconds(nextValue - i);
            i = nextValue;
        }
        // set to gray at the last, wait remainder of the total time
        if (i < totalTime)
        {
            if (rend)
                rend.material.color = Color.gray;
            yield return new WaitForSeconds(totalTime - i);
        }

        //this could be changed to recycling the object
        if (selfReference)
            Destroy(selfReference);
    }

    
        #endregion

    }
