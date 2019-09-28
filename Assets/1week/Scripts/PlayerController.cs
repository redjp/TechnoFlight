using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Prepare,
    Playing,
    Die,
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public static event System.Action<int, int> ScoreChanged = delegate { };
    public static event System.Action<int> HighscoreChanged = delegate { };

    [SerializeField]
    PlayerState _playerState;
    [SerializeField]
    float _fowardSpeed;
    [SerializeField]
    float _verticalInputScale;
    [SerializeField]
    float _horizontalInputScale;
    [SerializeField]
    float _xRotateScale;
    [SerializeField]
    float _zRotateScale;
    [SerializeField]
    [Range(0f, 1f)]
    float _forceCenterBack;
    [SerializeField]
    float _maxMoveX;
    [SerializeField]
    float _maxMoveY;
    //スコアカウント用
    [SerializeField]
    int _score;
    int _highscore;
    const string HIGHSCORE_PREF_KEY = "HIGHSCORE";

    [SerializeField]
    BirdManager _birdManager;
    [SerializeField]
    BackGround _background;

    #region Properties

    public PlayerState playerState
    {
        get { return _playerState; }
        private set { _playerState = value; }
    }
    public float fowardSpeed
    {
        get { return _fowardSpeed; }
        private set { _fowardSpeed = value; }
    }
    public int score
    {
        get { return _score; }
        private set
        {
            int sub = value - _score;
            _score = value;
            ScoreChanged(_score, (sub > 0) ? sub : 0);
        }
    }
    public int highscore
    {
        get { return _highscore; }
        private set
        {
            _highscore = value;
            PlayerPrefs.SetInt(HIGHSCORE_PREF_KEY, _highscore);
            HighscoreChanged(_highscore);
        }
    }
    #endregion

    Rigidbody _rigidbody;
    Transform _transform;
    Vector3 _targetPosition;


    void OnEnable()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
    }
    void OnDisable()
    {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
    }

    void GameManager_GameStateChanged(GameState newState, GameState oldState)
    {
        if (newState == GameState.Play)
        {
            InitPlayer();
        }
        if (newState == GameState.Movie)
        {
            StartCoroutine(MovieToResult());
        }
        if (newState == GameState.Result)
        {
            if (score > highscore)
            {
                highscore = score;
                naichilab.RankingLoader.Instance.SendScoreAndShowRanking(highscore);
            }
        }
    }

    IEnumerator MovieToResult()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.PlaySE(GameManager.Instance.SE_Shake);
        score += 10000 + _birdManager.birdCount * 1000;
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.ChangeState(GameState.Result);
    }

    void Start()
    {
        highscore = PlayerPrefs.GetInt(HIGHSCORE_PREF_KEY, 0);
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    void Update()
    {
        if (GameManager.Instance.gameState == GameState.Title ||
            (GameManager.Instance.gameState == GameState.Result && !naichilab.RankingLoader.Instance.isOpenRanking))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                naichilab.RankingLoader.Instance.SendScoreAndShowRanking(highscore);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                naichilab.UnityRoomTweet.Tweet("technoflight", string.Format("[#テクノフライト] あなたが突破した壁の枚数は{0}枚で、スコアは{1}です( ･ㅂ･)و", GameManager.Instance.currentStage, score), "unityroom", "unity1week");
            }
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.gameState == GameState.Play || GameManager.Instance.gameState == GameState.Title)
        {
            if (playerState == PlayerState.Playing)
            {
                var vert = Input.GetAxis("Vertical");
                var hori = Input.GetAxis("Horizontal");
                //少しずつ中央に戻る
                _targetPosition = new Vector3(_transform.position.x * (1 - _forceCenterBack),
                    _transform.position.y * (1 - _forceCenterBack), _transform.position.z + fowardSpeed);
                _targetPosition += Vector3.up * vert * _verticalInputScale + Vector3.right * hori * _horizontalInputScale;
                //範囲内に収める
                _targetPosition = new Vector3(Mathf.Clamp(_targetPosition.x, -_maxMoveX, _maxMoveX),
                    Mathf.Clamp(_targetPosition.y, -_maxMoveY, _maxMoveY), _targetPosition.z);

                //速度の変更
                var diff = _targetPosition - _transform.position;
                _rigidbody.velocity = diff * GameManager.Instance.gameSpeed;

                //向きを変える
                diff.y *= (diff.y > 0) ? 0.8f : 1;

                if (diff == Vector3.zero)
                    _rigidbody.rotation = Quaternion.identity * Quaternion.Euler(diff.y * -_xRotateScale, 0f, diff.x * -_zRotateScale);
                else
                    _rigidbody.rotation = Quaternion.LookRotation(diff) * Quaternion.Euler(diff.y * -_xRotateScale, 0f, diff.x * -_zRotateScale);
            }
        }
        else if (GameManager.Instance.gameState == GameState.Movie)
        {
            //少しずつ中央に戻る
            _targetPosition = new Vector3(_transform.position.x * 0.85f,
                    _transform.position.y * (1 - _forceCenterBack), _transform.position.z + fowardSpeed);
            //範囲内に収める
            _targetPosition = new Vector3(Mathf.Clamp(_targetPosition.x, -_maxMoveX, _maxMoveX),
                Mathf.Clamp(_targetPosition.y, -_maxMoveY, _maxMoveY), _targetPosition.z);

            //速度の変更
            var diff = _targetPosition - _transform.position;
            _rigidbody.velocity = diff;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (playerState == PlayerState.Playing)
        {
            GameManager.Instance.PlaySE(GameManager.Instance.SE_Hit);
            playerState = PlayerState.Die;
            _rigidbody.useGravity = true;

            Invoke("GameOver", 0.5f);
        }
    }

    void GameOver()
    {
        GameManager.Instance.ChangeState(GameState.Result);
    }

    void OnTriggerEnter(Collider other)
    {
        if (playerState == PlayerState.Playing)
        {
            GameManager.Instance.PlaySE(GameManager.Instance.SE_Shake);
            GameManager.Instance.PlaySE(GameManager.Instance.SE_Test);
            EZCameraShake.CameraShaker.Instance.ShakeOnce(5f, 5f, 0.1f, 0.4f);
            StartCoroutine(SpeedChanger());

            if (GameManager.Instance.hardMode == 0 && _birdManager.birdCount <= 0)
            {
                //ハードモードに切り替え
                GameManager.Instance.EnableHardMode(1);
            }

            //通過した枚数で難易度を上げる
            GameManager.Instance.ChangeStage(false);
            _background.ChangeRandomColor();

            score += ((_birdManager.birdCount > 0) ? _birdManager.birdCount + 25 : 60) * 5;
        }
    }

    IEnumerator SpeedChanger()
    {
        for (fowardSpeed = 30f; fowardSpeed > 10f; fowardSpeed -= 0.15f * GameManager.Instance.gameSpeed)
        {
            yield return new WaitForFixedUpdate();
        }
        fowardSpeed = 10f;
    }

    //ゲーム用に初期化
    void InitPlayer()
    {
        _transform.position = Vector3.zero;
        _rigidbody.useGravity = false;
        playerState = PlayerState.Playing;
        score = 0;
    }
}
