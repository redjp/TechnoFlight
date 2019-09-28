using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Title,
    Play,
    Movie,
    Result
}
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    //ゲームステートの変更イベント
    public static event System.Action<GameState, GameState> GameStateChanged = delegate { };

    //シングルトン

    [SerializeField]
    GameState _gameState;
    [SerializeField]
    float _gameSpeed;
    [SerializeField]
    float _holeSize;
    [SerializeField]
    int _currentStage;
    [SerializeField]
    int _hardMode;

    //サウンド
    [Header("Sound")]
    [SerializeField]
    AudioSource _bgmAudioSource;
    [SerializeField]
    AudioSource _seAudioSource;
    [SerializeField]
    AudioClip _SE_Shake;
    [SerializeField]
    AudioClip _SE_Hit;
    [SerializeField]
    AudioClip _SE_Break;
    [SerializeField]
    AudioClip _SE_Wind;
    [SerializeField]
    AudioClip _SE_Test;


    #region Properties

    public GameState gameState
    {
        get { return _gameState; }
        private set
        {
            if (value != _gameState)
            {
                GameState oldState = _gameState;
                _gameState = value;

                GameStateChanged(_gameState, oldState);
            }
        }
    }
    public float gameSpeed
    {
        get { return _gameSpeed; }
        private set { _gameSpeed = value; }
    }
    public float holeSize
    {
        get { return _holeSize; }
        private set { _holeSize = value; }
    }
    public int currentStage
    {
        get { return _currentStage; }
        private set { _currentStage = value; }
    }
    public int hardMode
    {
        get { return _hardMode; }
        private set { _hardMode = value; }
    }

    public AudioClip SE_Shake
    {
        get { return _SE_Shake; }
    }
    public AudioClip SE_Hit
    {
        get { return _SE_Hit; }
    }
    public AudioClip SE_Break
    {
        get { return _SE_Break; }
    }
    public AudioClip SE_Wind
    {
        get { return _SE_Wind; }
    }
    public AudioClip SE_Test
    {
        get { return _SE_Test; }
    }

    #endregion

    void Start()
    {
    }

    public void EnableHardMode(int mode)
    {
        PlaySE(SE_Wind);
        hardMode = mode;
    }
    public void ChangeStage(bool reset)
    {
        if (reset)
        {
            currentStage = 0;
            gameSpeed = 1f;
            holeSize = 2f;
        }
        else
        {
            currentStage++;
            gameSpeed = Mathf.Clamp(1f + (0.009f * currentStage), 1f, 1.4f);
            holeSize = Mathf.Clamp(2f - (0.025f * currentStage), 1f, 2f);

            if (hardMode < 2 && currentStage >= 40)
            {
                EnableHardMode(2);
            }
            if (currentStage >= 50)
            {
                //ゲームクリア
                PlaySE(SE_Break);
                ChangeState(GameState.Movie);
            }
        }
    }

    public void ChangeState(GameState newState)
    {
        gameState = newState;
        if (newState == GameState.Result)
        {
            DisableBGMLoop();
        }
    }

    public void PlayBGM()
    {
        _bgmAudioSource.Play();
        _bgmAudioSource.loop = true;
    }
    public void DisableBGMLoop()
    {
        _bgmAudioSource.loop = false;
    }
    public void PlaySE(AudioClip clip)
    {
        if (clip != null)
        {
            _seAudioSource.PlayOneShot(clip);
        }
    }

    public void SetVolume(int volume)
    {
        _bgmAudioSource.volume = volume / 100f;
        _seAudioSource.volume = volume / 100f;
    }

    public void StartGame()
    {
        ChangeStage(true);
        hardMode = 0;
        ChangeState(GameState.Play);
    }
}
