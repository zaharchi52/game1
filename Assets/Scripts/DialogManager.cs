using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    public GameObject dialogPanel;        // Canvas panel
    public Text dialogText;               // ���� � �������
    public GameObject choicesPanel;       // ������ � �������� ������
    public Button choiceButtonPrefab;     // prefab ������ ������

    private void Awake()
    {
        Instance = this;
        HideDialog();
    }

    public void ShowText(string text, Action onComplete = null, float autoCloseTime = 0f)
    {
        dialogPanel.SetActive(true);
        choicesPanel.SetActive(false);
        dialogText.text = text;
        if (autoCloseTime > 0f) StartCoroutine(AutoClose(autoCloseTime, onComplete));
        else StartCoroutine(WaitForClose(onComplete));
    }

    IEnumerator AutoClose(float t, Action onComplete)
    {
        yield return new WaitForSeconds(t);
        HideDialog();
        onComplete?.Invoke();
    }

    IEnumerator WaitForClose(Action onComplete)
    {
        // ���� ����� ����� ����� E ����� ������� (����� ������)
        while (!Input.GetKeyDown(KeyCode.E))
        {
            yield return null;
        }
        HideDialog();
        onComplete?.Invoke();
    }

    public void ShowChoices(string question, string[] choices, Action<int> onChoice)
    {
        dialogPanel.SetActive(true);
        choicesPanel.SetActive(true);
        dialogText.text = question;

        // ������� ������ ������
        foreach (Transform t in choicesPanel.transform) Destroy(t.gameObject);

        for (int i = 0; i < choices.Length; i++)
        {
            int index = i;
            Button b = Instantiate(choiceButtonPrefab, choicesPanel.transform);
            b.GetComponentInChildren<Text>().text = choices[i];
            b.onClick.AddListener(() =>
            {
                HideDialog();
                onChoice?.Invoke(index);
            });
        }
    }

    public void HideDialog()
    {
        dialogPanel.SetActive(false);
        choicesPanel.SetActive(false);
    }
}
