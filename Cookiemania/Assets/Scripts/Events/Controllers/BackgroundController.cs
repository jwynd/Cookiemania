using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public delegate void function();
    public function m_call;
    public Transform parentTransform;
    void Start()
    {
        //Fill Delegate
        //m_call = whateverwefillwith;
    }

    public void Init()
    {
        for(int i = 0; i < parentTransform.childCount; i++)
        {
            parentTransform.GetChild(i).gameObject.SetActive(true);
        }

        SetStage(0, m_call);
    }

    public void SetStage(int v, function m_call)
    {
        /*needs a public void SetStage(int v, Function<> callback) function that sets
        the scene to stage v and invokes the given callback when its ready for the next stage e.g.
        the movement has completed*/
    }

    public bool CanSetStage(int v)
    {
        // function that says whether a given v has a stage defined for it
        return true;
    }
}
