using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void ExitScreen()
    {
        if (PauseMenu.Instance)
            PauseMenu.Instance.Pause();
    }
}
