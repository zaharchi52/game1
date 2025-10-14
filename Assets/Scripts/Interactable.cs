using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [TextArea] public string promptText = "Нажми E чтобы взаимодействовать";
    public bool canInteract = true;

    // Этот метод вызывается при нажатии E игроком
    public virtual void Interact()
    {
        Debug.Log("Interact with " + name);
    }
}