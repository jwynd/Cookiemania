﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperManager : MonoBehaviour
{

    #region variables
    [System.Serializable]
    public class Prefabs
    {
        [Tooltip("Spawnable object prefabs")]
        public GameObject platform = null;
        [Tooltip("Weighted possibility to spawn (relative to other objects weights.) Prefabs in the middle of " +
            "the array are more likely due to using pseudo normal distribution.")]
        [Range(0.01f, 1.0f)]
        public float weight = 0.1f;
        [HideInInspector]
        public float probability = 0f;
    }

    
    [SerializeField]
    private Prefabs[] platformPrefabs = null;

    [Tooltip("How high the player can jump, used for determining how far platforms can be placed")]
    public float jumpHeight = 5.0f;
    [Tooltip("How far to either side from the offset position a platform can spawn")]
    public float width = 10.0f;
    [Tooltip("Minimum Height Increment")]
    public float minHeightIncrease = 0.0f;
    [Tooltip("Number of platforms per section")]
    public int density = 10;
    [Tooltip("Starting difficulty of the level")]
    public float startingDifficulty = 1.0f;
    [Tooltip("How much difficulty increases every time a region is built")]
    public float difficultyIncrement = 0.3f;
    [Tooltip("Height to finish the level")]
    public float heightGoal = 100f;
    [Tooltip("The rotation of the game object on the Z axis")]
    [Range(0.0f, 360.0f)]
    public float rotation = 0.0f;

    private float offset;
    private float height;
    

    private int max;
    private float weightRange = 0.0f;

    public JumperCameraController mainCamera { get; private set; }

    public JumperPlayerController player { get; private set; }

    public JumperPlatformTrigger trigger { get; private set; }

    public static JumperManager Instance { get; private set; }

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
        weightRange = 0;
        foreach (var fab in platformPrefabs)
        {
            fab.probability = fab.weight + weightRange;
            weightRange += fab.weight;
        }
        trigger = FindObjectOfType<JumperPlatformTrigger>().gameObject.GetComponent<JumperPlatformTrigger>();
        player = FindObjectOfType<JumperPlayerController>().gameObject.GetComponent<JumperPlayerController>();
        mainCamera = FindObjectOfType<JumperCameraController>().gameObject.GetComponent<JumperCameraController>();
    }


    void OnValidate() {
        if (transform.childCount != 0) {
            Debug.LogError("Ensure Spawner has no children");
        }
    }



    void Start()
    {
        
        if (!trigger || !player || !mainCamera || platformPrefabs.Length == 0) 
        { 
            Debug.LogError("need a player, trigger and camera with appropriate scripts" +
                " instantiated in the scene"); 
        }
        if (platformPrefabs[0].platform == null)
        {
            Debug.LogError("Platforms should not be null references");
        }
        GameObject g = Instantiate(platformPrefabs[0].platform, transform.position, Quaternion.identity);
        minHeightIncrease = player.gameObject.GetComponent<Renderer>().bounds.size.y * 2 + g.GetComponent<Renderer>().bounds.size.y * 2;
        Destroy(g);
        jumpHeight = player.GetJumpStrength() / 3;
        width = player.GetMaxVelocity() * 2.5f;
        Debug.Log(width);
        max  = (int)(density * 1.5);
        offset = player.transform.position.x;
        height = player.transform.position.y;
        BuildSection();
    }

    private GameObject Roulette()
    {
        //the 0 possibility is caught on startup
        if (platformPrefabs.Length < 2) { return platformPrefabs[0].platform; }
        float random = Random.Range(0, weightRange/2) + Random.Range(0, weightRange/2);
        foreach (var fab in platformPrefabs)
        {
            if (random < fab.probability)
            {
                return fab.platform;
            }
        }
        return platformPrefabs[platformPrefabs.Length - 1].platform;
    }

    #endregion

    #region publicFunctions

    public void BuildSection()
    {
        startingDifficulty += difficultyIncrement;
        for (int i = 0; i < density; ++i){
            float randomx = Random.Range(offset - width, offset + width);
            float randomy = Random.Range(height + minHeightIncrease, height + jumpHeight);
            Vector3 pos = new Vector3(randomx, randomy, 0);
            offset = pos.x;
            height = pos.y;
            GameObject g = Instantiate(Roulette(), pos, Quaternion.Euler(0, 0, rotation));
            g.transform.parent = transform;
            g.transform.SetAsFirstSibling();
            if (i == density - 3) trigger.transform.position = pos;
            if(transform.childCount > max) transform.GetChild(transform.childCount-1).gameObject.GetComponent<JumperPlatformController>().Remove();
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

    public static IEnumerator FlashThenKill(GameObject selfReference, float totalTime, float flashInterval, JumperPlatformAttachables killableChild = null)
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
