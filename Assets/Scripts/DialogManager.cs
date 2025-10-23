using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    public GameObject dialogPanel;        // Canvas panel
    public Text dialogText;               // поле с текстом
    public GameObject choicesPanel;       // панель с кнопками выбора
    public Button choiceButtonPrefab;     // prefab кнопки выбора

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
        // ждем когда игрок нажмёт E чтобы закрыть (можно менять)
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

        // очистка старых кнопок
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
