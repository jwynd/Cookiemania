using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.Events;

public class Desktop_EmailController : MonoBehaviour
{
    public static Desktop_EmailController Instance;
    public Vector3 nextEmailPosition;
    public GameObject emailPrefab;
    public float offset;
    private GameObject emailsParent;
    private static List<GameObject> emails = new List<GameObject>();

    void OnValidate(){
        Assert.IsNotNull(emailPrefab);
        Assert.IsNotNull(emailPrefab.transform.GetChild(0).gameObject.GetComponent<Text>());
        Assert.IsNotNull(emailPrefab.GetComponent<General_Email>());
    }

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance != null && Instance != this){
          Debug.LogError("Attempting to create second instance of Singleton pattern: Desktop_EmailController");
          Destroy(this);
        }
        Instance = this;
        emailsParent = GameObject.Find("Canvas/EmailsParent");
        emailPrefab.SetActive(false);
    }

    public void spawnEmail(string subject, string body, string[] responses, UnityAction[] responseActions){
        GameObject e = Instantiate(emailPrefab, Vector3.zero, Quaternion.identity);
        e.transform.GetChild(0).gameObject.GetComponent<Text>().text = subject;
        e.GetComponent<General_Email>().bodyText = body;
        e.GetComponent<RectTransform>().position = nextEmailPosition;
        e.transform.SetParent(emailsParent.transform, false);
        e.SetActive(true);
        emails.Add(e);
        nextEmailPosition = new Vector3(nextEmailPosition.x, nextEmailPosition.y - offset, nextEmailPosition.z);
    }

    // for testing purposes only
    // void Update(){
    //     if(Input.GetKeyDown(KeyCode.Space)){
    //       spawnEmail("Test Email", "This is a test");
    //     }
    // }
}
