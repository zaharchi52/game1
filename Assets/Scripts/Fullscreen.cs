using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fullscreen : MonoBehaviour
{
    public void ChangeScreenMode()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Debug.Log("Режим экрана изменён");
    }
}
