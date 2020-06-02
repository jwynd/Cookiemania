using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat_PlayerMove : MonoBehaviour
{
    private bool canMove = true;
    // Update is called once per frame
    void Update()
    {
        if(canMove && Input.GetKeyDown(KeyCode.Mouse0)){
            this.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
        }
    }

    public void ForbidMove(){
        canMove = false;
    }

    public void AllowMove(){
        canMove = true;
    }
}
