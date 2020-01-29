using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_PlatformGeneration : MonoBehaviour
{
    [Tooltip("This game object will be coppied to create platforms")]
    public GameObject platform;
    [Tooltip("An object that will call BuildSection when entered as a trigger")]
    public GameObject trigger;
    [Tooltip("How high the player can jump, used for determining how far platforms can be placed")]
    public float jumpHeight = 5.0f;
    [Tooltip("Below this height, no platforms can spawn")]
    public float height;
    [Tooltip("How far to either side from the offset position a platform can spawn")]
    public float width = 10.0f;
    [Tooltip("Center of the platform collumn")]
    public float offset = 0.0f;
    [Tooltip("Number of platforms per section")]
    public int density = 20;
    [Tooltip("The rotation of the game object on the Z axis")]
    [Range(0.0f, 360.0f)]
    public float rotation = 0.0f;

    private max = density * 2;
    void OnValidate(){
        if(this.transform.childCount != 0){
            Debug.LogError("Ensure Spawner has no children");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        BuildSection();
    }

    public void BuildSection(){
        for(int i = 0; i < density; ++i){
            Vector3 pos = new Vector3(Random.Range(offset-width, offset+width), Random.Range(height, height+jumpHeight), 0);
            height = pos.y;
            GameObject g = Instantiate(platform, pos, Quaternion.Euler(0, 0, rotation));
            g.transform.parent = this.transform;
            g.transform.SetAsFirstSibling();
            if(i == density - 1) Instantiate(trigger, pos, Quaternion.identity);
            if(this.transform.childCount > max) Destroy(this.transform.Child(this.transform.childCount-1));
        }
    }
    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
