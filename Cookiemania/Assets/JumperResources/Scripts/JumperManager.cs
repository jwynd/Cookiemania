using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperManager : MonoBehaviour
{
    [Tooltip("This game object will be copied to create platforms")]
    public GameObject platform;
    [Tooltip("How high the player can jump, used for determining how far platforms can be placed")]
    public float jumpHeight = 5.0f;
    [Tooltip("Below this height, no platforms can spawn")]
    public float height;
    [Tooltip("How far to either side from the offset position a platform can spawn")]
    public float width = 10.0f;
    [Tooltip("Center of the platform collumn")]
    public float offset = 0.0f;
    [Tooltip("Minimum Height Increment")]
    public float minHeightIncrease = 0.0f;
    [Tooltip("Number of platforms per section")]
    public int density = 10;
    [Tooltip("The rotation of the game object on the Z axis")]
    [Range(0.0f, 360.0f)]
    public float rotation = 0.0f;

    private float additionalHeightToNextPlatform = 0.0f;

    private int max;
    public GameObject mainCamera { get; private set; }

    public GameObject player { get; private set; }

    public GameObject trigger { get; private set; }

    public static JumperManager Instance { get; private set; }


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
        BuildSection();
    }

    public void BuildSection(){
        for(int i = 0; i < density; ++i){
            float x = Random.Range(offset - width, offset + width);
            Vector3 pos = new Vector3(Random.Range(offset-width, offset+width), Random.Range(height+minHeightIncrease, height+jumpHeight), 0);
            offset = pos.x;
            height = pos.y;
            GameObject g = Instantiate(platform, pos, Quaternion.Euler(0, 0, rotation));
            g.transform.parent = transform;
            g.transform.SetAsFirstSibling();
            if (i == density - 1) trigger.transform.position = pos;
            if(transform.childCount > max) Destroy(transform.GetChild(transform.childCount-1).gameObject);
        }
    }
}
