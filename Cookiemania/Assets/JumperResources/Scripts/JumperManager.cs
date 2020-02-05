using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperManager : MonoBehaviour
{

    #region variables
    [Tooltip("This game object will be copied to create platforms")]
    public GameObject platform;
    [Tooltip("How high the player can jump, used for determining how far platforms can be placed")]
    public float jumpHeight = 5.0f;
    [Tooltip("How far to either side from the offset position a platform can spawn")]
    public float width = 10.0f;
    [Tooltip("Minimum Height Increment")]
    public float minHeightIncrease = 0.0f;
    [Tooltip("Number of platforms per section")]
    public int density = 10;
    [Tooltip("The rotation of the game object on the Z axis")]
    [Range(0.0f, 360.0f)]
    public float rotation = 0.0f;

    private float offset;
    private float height;
    private float difficulty = 1.0f;

    private int max;
    public GameObject mainCamera { get; private set; }

    public GameObject player { get; private set; }

    public GameObject trigger { get; private set; }

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

    }

    
    void OnValidate(){
        if(transform.childCount != 0){
            Debug.LogError("Ensure Spawner has no children");
        }
    }



    void Start()
    {
        trigger = FindObjectOfType<JumperPlatformTrigger>().gameObject;
        player = FindObjectOfType<JumperPlayerController>().gameObject;
        mainCamera = FindObjectOfType<JumperCameraController>().gameObject;
        if (!trigger || !player || !mainCamera) 
        { 
            Debug.LogError("need a player, trigger and camera with appropriate scripts" +
                " instantiated in the scene"); 
        }
        GameObject g = Instantiate(platform, transform.position, Quaternion.identity);
        minHeightIncrease = player.GetComponent<Collider2D>().bounds.extents.y * 2 + g.GetComponent<Collider2D>().bounds.extents.y * 2;
        Destroy(g);
        max  = (int)(density * 1.5);
        offset = player.transform.position.x;
        height = player.transform.position.y;
        BuildSection();
    }

    #endregion

    #region publicFunctions

    public void BuildSection(){
        Vector3 pos = new Vector3(offset, height, 0);
        for (int i = 0; i < density; ++i){
            float xprev = pos.x;
            pos = new Vector3(Random.Range(offset-width, offset+width), Random.Range(height+minHeightIncrease, height+jumpHeight), 0);
            offset = pos.x;
            height = pos.y;
            GameObject g = Instantiate(platform, pos, Quaternion.Euler(0, 0, rotation));
            g.GetComponent<JumperPlatformController>().previousLedgeDirection = pos.x - xprev > 0 ? 1 : -1;
            g.transform.parent = transform;
            g.transform.SetAsFirstSibling();
            if (i == density - 3) trigger.transform.position = pos;
            if(transform.childCount > max) transform.GetChild(transform.childCount-1).gameObject.GetComponent<JumperPlatformController>().Remove();
        }
    }

    public float GetDifficultyMultiplier()
    {
        return difficulty;
    }

    #endregion
}
