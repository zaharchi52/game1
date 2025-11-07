using UnityEngine;

public class Interactable : MonoBehaviour
{
    [TextArea] public string promptText = "����� E ����� �����������������";
    public bool canInteract = true;

    // ���� ����� ���������� ��� ������� E �������
    public virtual void Interact()
    {
        Debug.Log("Interact with " + name);
    }
}
