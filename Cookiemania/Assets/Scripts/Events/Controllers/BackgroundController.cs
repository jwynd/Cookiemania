
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BackgroundController : MonoBehaviour
{
    public delegate void function();
    public function m_call;
    public Transform parentTransform;
    public Animation ani;
    public Animator aniCtrl;
    public Transform loc1;
    Dictionary<int, function> Stages;
    void Start()
    {
        Stages = new Dictionary<int, function>();
        //Fill Delegate
        //m_call = whateverwefillwith;
    }

    public void Init()
    {
        //activate stored game object
        //check for @ initial position
        for(int i = 0; i < parentTransform.childCount; i++)
        {
            parentTransform.GetChild(i).gameObject.SetActive(true);
        }

        SetStage(0, m_call);
    }

    public void SetStage(int v, function callback)
    {
        /*needs a public void SetStage(int v, Function<> callback) function that sets
        the scene to stage v and invokes the given callback when its ready for the next stage e.g.
        the movement has completed*/
        if (CanSetStage(v) == true)
        {
            Stages.Add(v, callback);
            callback();
        } else
        {
            Debug.Log("Stage could not be set");
        }
        
    }

    public bool CanSetStage(int v)
    {
        // function that says whether a given v has a stage defined for it
        bool canSet = true;
        if (Stages.TryGetValue(v, out function entry) == true)
        {
            //Value exists. stage either does not need to be set or we can override in that case canSet should be true and default should be false
            canSet = false;
        }
        return canSet;
    }
}
