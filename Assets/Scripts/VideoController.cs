using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class VideoController : MonoBehaviour
{
    [Header("Настройки видео")]
    [SerializeField] private List<VideoClip> videoClips; // Список видео для воспроизведения
    [SerializeField] private VideoPlayer videoPlayer; // Компонент VideoPlayer
    [SerializeField] private RawImage videoDisplay; // RawImage для отображения видео

    [Header("Настройки субтитров")]
    [SerializeField] private List<string> subtitles; // Текст субтитров для каждого видео
    [SerializeField] private Text subtitleText; // UI Text для отображения субтитров
    [SerializeField] private GameObject subtitlePanel; // Панель субтитров (опционально)

    [Header("Временные настройки")]
    [SerializeField] private float delayBetweenVideos = 0; // Задержка между видео
    [SerializeField] private float subtitleFadeTime = 0.25f; // Время появления/исчезновения субтитров

    [Header("Автопереход")]
    [SerializeField] private bool autoSkipToNextScene = true; // Автоматически переходить к следующей сцене
    [SerializeField] private string nextSceneName; // Имя следующей сцены

    private int currentVideoIndex = 0;
    private bool isPlaying = false;
    private CanvasGroup subtitleCanvasGroup;

    void Start()
    {
        // Инициализация
        InitializeComponents();

        // Начинаем воспроизведение
        StartCoroutine(PlayVideoSequence());
    }

    void InitializeComponents()
    {
        // Настройка VideoPlayer
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoDisplay != null && videoPlayer != null)
        {
            videoPlayer.targetTexture = new RenderTexture((int)videoDisplay.rectTransform.rect.width,
                                                         (int)videoDisplay.rectTransform.rect.height, 24);
            videoDisplay.texture = videoPlayer.targetTexture;
        }

        // Настройка субтитров
        if (subtitlePanel != null)
        {
            subtitleCanvasGroup = subtitlePanel.GetComponent<CanvasGroup>();
            if (subtitleCanvasGroup == null)
                subtitleCanvasGroup = subtitlePanel.AddComponent<CanvasGroup>();
        }

        // Скрываем субтитры в начале
        if (subtitleCanvasGroup != null)
            subtitleCanvasGroup.alpha = 0;
    }

    IEnumerator PlayVideoSequence()
    {
        isPlaying = true;

        for (currentVideoIndex = 0; currentVideoIndex < videoClips.Count; currentVideoIndex++)
        {
            // Загружаем текущее видео
            videoPlayer.clip = videoClips[currentVideoIndex];
            videoPlayer.Prepare();

            // Ждем пока видео подготовится
            while (!videoPlayer.isPrepared)
                yield return null;

            // Показываем субтитры для этого видео (если они есть)
            if (currentVideoIndex < subtitles.Count && !string.IsNullOrEmpty(subtitles[currentVideoIndex]))
            {
                subtitleText.text = subtitles[currentVideoIndex];
                yield return StartCoroutine(FadeSubtitles(true));
            }

            // Начинаем воспроизведение
            videoPlayer.Play();

            // Ждем окончания видео
            yield return new WaitForSeconds((float)videoPlayer.clip.length);

            // Скрываем субтитры перед следующим видео
            if (currentVideoIndex < subtitles.Count && !string.IsNullOrEmpty(subtitles[currentVideoIndex]))
            {
                yield return StartCoroutine(FadeSubtitles(false));
            }

            // Задержка между видео
            if (currentVideoIndex < videoClips.Count - 1)
                yield return new WaitForSeconds(delayBetweenVideos);
        }

        // Завершение последовательности
        OnSequenceComplete();
    }
    IEnumerator FadeSubtitles(bool fadeIn)
    {
        if (subtitleCanvasGroup == null) yield break;

        float targetAlpha = fadeIn ? 1f : 0f;
        float startAlpha = subtitleCanvasGroup.alpha;
        float timer = 0f;

        while (timer < subtitleFadeTime)
        {
            timer += Time.deltaTime;
            subtitleCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / subtitleFadeTime);
            yield return null;
        }

        subtitleCanvasGroup.alpha = targetAlpha;
    }

    void OnSequenceComplete()
    {
        isPlaying = false;
        Debug.Log("Интро завершено!");

        // Автоматический переход к следующей сцене
        if (autoSkipToNextScene && !string.IsNullOrEmpty(nextSceneName))
        {
            StartCoroutine(LoadNextSceneWithDelay(1f));
        }
    }

    IEnumerator LoadNextSceneWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    // Метод для пропуска интро (можно привязать к кнопке)
    public void SkipIntro()
    {
        if (isPlaying)
        {
            StopAllCoroutines();

            if (subtitleCanvasGroup != null)
                subtitleCanvasGroup.alpha = 0;

            OnSequenceComplete();
        }
    }

    // Метод для перехода к конкретному видео (для отладки)
    public void PlayVideoAtIndex(int index)
    {
        if (index >= 0 && index < videoClips.Count)
        {
            StopAllCoroutines();
            currentVideoIndex = index;
            StartCoroutine(PlayVideoSequence());
        }
    }
}