using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleRoomController : MonoBehaviour
{
    public PuzzlePassage[] passages;    // 4 прохода
    public Sprite normalSprite;         // спрайт обычного прохода
    public Sprite correctSprite;        // спрайт правильного прохода

    private int currentStage = 0;       // номер шага (0,1,2)
    private int correctPassageIndex;    // какой сейчас правильный

    private void Start()
    {
        ChooseNewCorrectPassage();
    }

    // выбрать один из 4 проходов как правильный
    private void ChooseNewCorrectPassage()
    {
        // снимаем выделение
        foreach (var p in passages)
            p.SetNormal(normalSprite);

        correctPassageIndex = Random.Range(0, passages.Length);
        passages[correctPassageIndex].SetCorrect(correctSprite);

        Debug.Log("Новый правильный проход: " + passages[correctPassageIndex].name);
    }

    // вызывается, когда игрок входит в проход
    public void PlayerEntered(PuzzlePassage passage)
    {
        if (passage == passages[correctPassageIndex])
        {
            Debug.Log("Игрок вошёл в правильный проход!");

            currentStage++;

            // если 3 раза успешно — завершение
            if (currentStage >= 3)
            {
                Debug.Log("ГОЛОВОЛОМКА РЕШЕНА!");
                SceneManager.LoadScene("MainMenu");  // заменишь
                return;
            }

            // выбираем новый правильный проход
            ChooseNewCorrectPassage();
        }
        else
        {
            Debug.Log("Неправильный проход. Игрок возвращён.");
            // ничего не делаем — игрок просто телепортнётся назад
        }
    }
}
