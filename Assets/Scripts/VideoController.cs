using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;

using Button = UnityEngine.UI.Button;
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

    [Header("Кнопка в конце")]
    [SerializeField] private Button endButton; // Кнопка, которая появится после видео
    [SerializeField] private float buttonAppearDelay = 1f; // Задержка перед появлением кнопки

    [Header("Временные настройки")]
    [SerializeField] private float delayBetweenVideos = 0; // Задержка между видео
    [SerializeField] private float subtitleFadeTime = 0.25f; // Время появления/исчезновения субтитров

    private int currentVideoIndex = 0;
    private bool isPlaying = false;
    private CanvasGroup subtitleCanvasGroup;
    private CanvasGroup buttonCanvasGroup;

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

        SetupEndButton();
    }
void SetupEndButton()
{
    // Скрываем кнопку в начале
    if (endButton != null)
    {
        endButton.gameObject.SetActive(false);

        // Добавляем CanvasGroup для плавного появления
        buttonCanvasGroup = endButton.GetComponent<CanvasGroup>();
        if (buttonCanvasGroup == null)
            buttonCanvasGroup = endButton.gameObject.AddComponent<CanvasGroup>();
        buttonCanvasGroup.alpha = 0;
    }
    else if (endButton != null)
    {
        endButton.gameObject.SetActive(false);

        buttonCanvasGroup = endButton.GetComponent<CanvasGroup>();
        if (buttonCanvasGroup == null)
            buttonCanvasGroup = endButton.gameObject.AddComponent<CanvasGroup>();
        buttonCanvasGroup.alpha = 0;
    }

    // Назначаем действие на кнопку
    if (endButton != null)
    {
        endButton.onClick.RemoveAllListeners(); // Очищаем старые события
        endButton.onClick.AddListener(OnEndButtonClicked);
    }
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

        StartCoroutine(ShowEndButtonWithDelay());
    }

    IEnumerator ShowEndButtonWithDelay()
    {
        // Ждем указанное время
        yield return new WaitForSeconds(buttonAppearDelay);

        // Активируем объект кнопки
        if (endButton != null)
        {
            endButton.gameObject.SetActive(true);
            yield return StartCoroutine(FadeButton(true));
        }
    }

    IEnumerator FadeButton(bool fadeIn)
    {
        if (buttonCanvasGroup == null) yield break;

        float targetAlpha = fadeIn ? 1f : 0f;
        float startAlpha = buttonCanvasGroup.alpha;
        float timer = 0f;
        float fadeDuration = 0.5f; // Время появления кнопки

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            buttonCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }

        buttonCanvasGroup.alpha = targetAlpha;

        // Если скрываем кнопку, деактивируем объект
        if (!fadeIn)
        {
            if (endButton != null) endButton.gameObject.SetActive(false);
        }
    }

    // Метод, который вызывается при нажатии на кнопку
    void OnEndButtonClicked()
    {
        Debug.Log("Кнопка 'Продолжить' нажата!");

        // Плавно скрываем кнопку
        StartCoroutine(HideButton());
    }

    IEnumerator HideButton()
    {
        // Плавно скрываем кнопку
        yield return StartCoroutine(FadeButton(false));
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