using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desktop_EmailManager : MonoBehaviour
{
    public GameObject EmailTemplate;
    public static Desktop_EmailManager Instance {get; private set;}
    // Start is called before the first frame update
    void Awake()
    {
        if(Instance != null && Instance != this){
            Destroy(this.gameObject);
        }
        Instance = this;
    }

    void newEmail(Vector3 pos, string subject, string body, string[] responses){
        GameObject e = Instantiate(EmailTemplate, pos, Quaternion.identity);
        Desktop_Email email = e.GetComponent<Desktop_Email>();
        email.subject = subject;
        email.body = body;
        email.SetResponses(responses);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
