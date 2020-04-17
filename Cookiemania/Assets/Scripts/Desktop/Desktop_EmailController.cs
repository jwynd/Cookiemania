using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.Events;

public class Desktop_EmailController : MonoBehaviour
{
    public static Desktop_EmailController Instance;
    public Transform[] emailPositions;
    public GameObject newEmail;
    public GameObject emailLiteral;
    public GameObject[] disableOnEmailClick;
    public GameObject[] enableOnEmailClick;
    private GameObject emailsParent;
    private static List<GameObject> emails = new List<GameObject>();
    private int spotIndex = 0;
    private bool[] occupied;

    void OnValidate(){
        Assert.IsNotNull(newEmail);
        // Assert.IsNotNull(emailPrefab.transform.GetChild(0).gameObject.GetComponent<Text>());
        Assert.IsNotNull(newEmail.GetComponent<Email_Click>());
    }

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance != null && Instance != this){
          Debug.LogError("Attempting to create second instance of Singleton pattern: Desktop_EmailController");
          Destroy(this);
        }
        Instance = this;
        emailsParent = GameObject.Find("Email");
        occupied = new bool[emailPositions.Length];
        for(int i = 0; i < occupied.Length; ++i) occupied[i] = false;
    }

    public void spawnEmail(string subject, string body, string[] responseNames, UnityAction[] responseActions){
        GameObject e; // this will be the new email
        if(spotIndex < emailPositions.Length){
            e = Instantiate(newEmail, new Vector3(emailPositions[spotIndex].position.x, emailPositions[spotIndex].position.y, emailPositions[spotIndex].position.z), Quaternion.identity);
            e.transform.SetParent(emailsParent.transform, false);
            e.SetActive(true);
            emails.Add(e);
            occupied[spotIndex] = true;
        } else {
            e = Instantiate(newEmail, Vector3.zero, Quaternion.identity);
            e.transform.SetParent(emailsParent.transform, false);
            e.SetActive(false);
            emails.Add(e);
        }
        Email_Click ec = e.GetComponent<Email_Click>();
        ec.disable = disableOnEmailClick;
        ec.enable = enableOnEmailClick;
        ec.email = emailLiteral;
        ec.names = responseNames;
        ec.responses = responseActions;
        ec.subject = subject;
        ec.body = body;
    }

    // this function should be called in Email_Clicks onDestroy
    public void reorderEmails(){
      // Debug.LogError("Reorder emails is not implemented");
      foreach(GameObject e in emails){
        if(e == null) continue;
        // e.transform.position = nextEmailPosition;
        // nextEmailPosition = new Vector3(nextEmailPosition.x, nextEmailPosition.y - offset, nextEmailPosition.z);
      }
    }

    void money(){
      General_Score.Instance.money += 20;
    }

    void trust(){
      General_Score.Instance.trust += 20;
    }

    void both(){
      money();
      trust();
    }

    // for testing purposes only
    void Update(){
        if(Input.GetKeyDown(KeyCode.E)){
          spawnEmail("Test Email", "This is a test", new string[] {"Money", "Trust", "Both"}, new UnityAction[] {money, trust, both});
        }
    }
}
