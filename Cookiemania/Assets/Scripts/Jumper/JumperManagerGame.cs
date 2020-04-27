using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public float probability { get; set; } = 0f;
    }

    [SerializeField]
    private List<Platforms> nightPrefabs = new List<Platforms>();
    [SerializeField]
    private List<Platforms> dayPrefabs = new List<Platforms>();
    [SerializeField]
    private GameObject nightExit = null;
    [SerializeField]
    private GameObject dayExit = null;
    [SerializeField]
    private JumperSemiPermanentPlatforms startingPlatform = null;
    [SerializeField]
    private Sprite defaultDayPlatformImage = null;

    [Tooltip("How far to either side from the offset position a platform can spawn")]
    public float width = 10.0f;

    [Tooltip("Number of platforms per section")]
    public int density = 10;
    [Range(1f, 45f)]
    [Tooltip("Starting difficulty of the level")]
    public float startingDifficulty = 1.0f;
    [Tooltip("How much difficulty increases every time a region is built")]
    public float difficultyIncrement = 0.3f;
    [Tooltip("Bonus for beating the level")]
    public float levelReward = 40f;
    [Tooltip("Height to finish the level")]
    public float heightGoal = 100f;
    [Tooltip("Maximum height player can fall")]
    public float maxFallDistance = 15f;
    [Tooltip("The rotation of the game object on the Z axis")]
    [Range(0.0f, 360.0f)]
    public float rotation = 0.0f;
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
    public bool Tutorial { get; protected set; } = true;
    public bool Night { get; protected set; } = false;

    //game manager always tagged with game controller
    private const string myTag = "GameController";
    private float minHeightIncrease = 0.0f;

    private float offset;
    private float height;
    private float maxHeightReached;
    private bool canPlaceAboveLast = true;
    private float firstPrefabWidth = 0f;
    private bool haveBuiltExit = false;
    private int max;
    private float weightRange = 0.0f;
    private float checkAgainstPosition;
    private float jumpHeight;
    private List<Platforms> actualPrefabs;
    private GameObject actualExit;

    public JumperCameraController MainCam { get; private set; }

    public JumperPlayerController Player { get; private set; }

    public static JumperManagerGame Instance { get; private set; }

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
        //look to actual game manager to update outer game variables and stuff
        RunAwakeInit();
    }

    private void RunAwakeInit()
    {
        weightRange = 0;
        PrefabInit();
        Player = FindObjectOfType<JumperPlayerController>().gameObject.GetComponent<JumperPlayerController>();
        MainCam = FindObjectOfType<JumperCameraController>().gameObject.GetComponent<JumperCameraController>();
        heightGoal *= startingDifficulty;
    }

    private void PrefabInit()
    {
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
        width = Player.GetMaxVelocity() * 2.5f;
        Debug.Log(width);
        max  = (int)(density * 1.5);
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

    #endregion

    #region update
    private void FixedUpdate()
    {
        if (Player == null) { return; }
        float heightRightNow = Player.transform.position.y;
        maxHeightReached = heightRightNow > maxHeightReached ? heightRightNow : maxHeightReached;
        //not checking for health in update, will just run end on player death
        //else if (player.GetCurrentHealth() <= 0)
        //{
         //   JumperUIManager.Instance.End(false);
        //}
        if (maxHeightReached - maxFallDistance >= heightRightNow)
        {
            JumperManagerUI.Instance.End(false, false);
        }
        if (heightRightNow > checkAgainstPosition)
        { 
            BuildSection();
        }
    }

    private void AttemptToBuildExit()
    {
        if (haveBuiltExit) { return; }
        float randomx = BuildHelper();
        float randomy = UnityEngine.Random.Range(height + minHeightIncrease, height + jumpHeight);
        Vector3 pos = new Vector3(randomx, randomy, 0);
        Instantiate(actualExit, pos, Quaternion.Euler(0, 0, rotation));
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
                UnityEngine.Random.Range(height + (minHeightIncrease * bounceHeightMultiplier), height + (jumpHeight * bounceHeightMultiplier));
            if (randomy >= heightGoal)
            {
                AttemptToBuildExit();
                break;
            }
            float randomx = BuildHelper();
            Vector3 pos = new Vector3(randomx, randomy, 0);
            offset = pos.x;
            height = pos.y;
            GameObject g = Instantiate(Roulette(), pos, Quaternion.Euler(0, 0, rotation));
            canPlaceAboveLast = g.GetComponent<JumperGeneralPlatform>().CanPlacePlatformsAbove();
            g.transform.parent = transform;
            g.transform.SetAsFirstSibling();
            if (i == density - 3)
            {
                checkAgainstPosition = pos.y;
            }
            if(transform.childCount > max) transform.GetChild(transform.childCount-1).gameObject.GetComponent<JumperGeneralPlatform>().Remove();
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

    public float GetLevelReward()
    {
        return levelReward;
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

    public bool IsRunningTutorial()
    {
        return Tutorial;
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

    public static IEnumerator FlashThenKill(GameObject selfReference, float totalTime, float flashInterval, JumperGeneralThreat killableChild = null)
    {
        if (killableChild != null) { killableChild.PlatformDestroyed(totalTime, flashInterval); }
        int timer = (int)(totalTime / flashInterval);
        Renderer rend = selfReference.GetComponent<Renderer>();
        for (int i = 0; i < timer; i++)
        {
            rend.material.color = Color.gray;
            yield return new WaitForSeconds(flashInterval);
            rend.material.color = Color.white;
            yield return new WaitForSeconds(flashInterval);
        }
        //this could be changed to recycling the object
        Destroy(selfReference.gameObject);
    }

    
        #endregion


    }
