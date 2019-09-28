using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject[] ui_OnTitle;
    public GameObject[] ui_OnPlay;
    public GameObject[] ui_OnMovie;
    public GameObject[] ui_OnResult;

    public TextMeshProUGUI dynamicScore;
    public TextMeshProUGUI dynamicScorePlus;
    public TextMeshProUGUI dynamicStage;
    public TextMeshProUGUI dynamicVolume;

    int _currentVolume;

    void OnEnable()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
        PlayerController.ScoreChanged += PlayerController_ScoreChanged;
    }
    void OnDisable()
    {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
        PlayerController.ScoreChanged -= PlayerController_ScoreChanged;
    }

    void GameManager_GameStateChanged(GameState newState, GameState oldState)
    {
        switch (newState)
        {
            case GameState.Title:
                ResetUI();
                TitleUI();
                break;
            case GameState.Play:
                ResetUI();
                PlayUI();
                break;
            case GameState.Movie:
                MovieUI();
                break;
            case GameState.Result:
                ResultUI();
                break;
        }
    }
    void PlayerController_ScoreChanged(int newScore, int sub)
    {
        dynamicScore.text = newScore.ToString("00000");
        if (sub > 0)
        {
            dynamicScorePlus.text = (sub == 300) ? "BONUS!! +300" : (sub == 250) ? "PERFECT +250" : string.Format("+{0}", sub);
            StartCoroutine(ShowScorePlus());
        }

        dynamicStage.text = string.Format("{0}/50", GameManager.Instance.currentStage);
    }

    IEnumerator ShowScorePlus()
    {
        dynamicScorePlus.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        dynamicScorePlus.gameObject.SetActive(false);
    }

    void Start()
    {
        _currentVolume = PlayerPrefs.GetInt("VOLUME", 100);
        dynamicVolume.text = string.Format("{0}%", _currentVolume);
        GameManager.Instance.SetVolume(_currentVolume);
        ResetUI();
        TitleUI();
    }

    void Update()
    {
        if (GameManager.Instance.gameState == GameState.Title ||
            (GameManager.Instance.gameState == GameState.Result && !naichilab.RankingLoader.Instance.isOpenRanking))
        {
            if (Input.GetButtonDown("Submit"))
            {
                GameManager.Instance.PlayBGM();
                GameManager.Instance.StartGame();
            }
        }
    }

    void ResetUI()
    {
        foreach (var ui in ui_OnTitle)
        {
            ui.SetActive(false);
        }
        foreach (var ui in ui_OnPlay)
        {
            ui.SetActive(false);
        }
        foreach (var ui in ui_OnMovie)
        {
            ui.SetActive(false);
        }
        foreach (var ui in ui_OnResult)
        {
            ui.SetActive(false);
        }

        dynamicScore.gameObject.SetActive(false);
        dynamicScorePlus.gameObject.SetActive(false);
        dynamicStage.gameObject.SetActive(false);
        dynamicVolume.gameObject.SetActive(false);
    }

    void TitleUI()
    {
        foreach (var ui in ui_OnTitle)
        {
            ui.SetActive(true);
        }
        dynamicVolume.gameObject.SetActive(true);
    }

    void PlayUI()
    {
        foreach (var ui in ui_OnPlay)
        {
            ui.SetActive(true);
        }

        dynamicScore.gameObject.SetActive(true);
        dynamicStage.gameObject.SetActive(true);
    }

    void MovieUI()
    {
        foreach (var ui in ui_OnMovie)
        {
            ui.SetActive(true);
        }
    }

    void ResultUI()
    {
        foreach (var ui in ui_OnResult)
        {
            ui.SetActive(true);
        }
        dynamicVolume.gameObject.SetActive(true);
    }

    public void VolumeDown()
    {
        if (GameManager.Instance.gameState != GameState.Play)
        {
            _currentVolume = (_currentVolume < 20) ? 0 : _currentVolume - 20;
            dynamicVolume.text = string.Format("{0}%", _currentVolume);
            GameManager.Instance.SetVolume(_currentVolume);
            GameManager.Instance.PlaySE(GameManager.Instance.SE_Shake);
            PlayerPrefs.SetInt("VOLUME", _currentVolume);
        }
    }

    public void VolumeUp()
    {
        _currentVolume = (_currentVolume > 80) ? 100 : _currentVolume + 20;
        dynamicVolume.text = string.Format("{0}%", _currentVolume);
        GameManager.Instance.SetVolume(_currentVolume);
        GameManager.Instance.PlaySE(GameManager.Instance.SE_Shake);
        PlayerPrefs.SetInt("VOLUME", _currentVolume);
    }
}
