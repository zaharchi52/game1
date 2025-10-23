using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ����� �����������
    public bool puzzleCyclicSolved = false;
    public bool puzzleRaspberrySolved = false;
    public bool puzzleSphinxSolved = false;

    // ���� ����
    public bool hasKeySphere = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
