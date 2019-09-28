using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{

    [SerializeField]
    Transform _playerTrans;
    [SerializeField]
    GameObject _wallPrefab;
    [SerializeField]
    float _spawnInterval;

    GameObject[] _walls;
    const int MAX_WALLS = 2;
    const float START_OFFSET = 10f;
    float _prebPoint;
    int _oldid;

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
            InitSpawner();
        }
    }
    void Start()
    {
        _prebPoint = START_OFFSET + _spawnInterval;
        _walls = new GameObject[MAX_WALLS];
        for (int i = 0; i < MAX_WALLS; i++)
        {
            _walls[i] = Instantiate(_wallPrefab, new Vector3(0f, 0f, -100f), Quaternion.identity);
            _walls[i].transform.parent = this.transform;
        }
        _oldid = 0;
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.gameState == GameState.Play)
        {
            //50枚で生成をやめる（暫定）
            if (_playerTrans.position.z > _spawnInterval + _prebPoint && GameManager.Instance.currentStage < 49)
            {
                //壁を生成
                _walls[_oldid].transform.localPosition = new Vector3(0f, 0f, _prebPoint + _spawnInterval * MAX_WALLS);
                _walls[_oldid].GetComponent<WallController>().SetWall();

                _prebPoint += _spawnInterval;
                _oldid = (_oldid + 1 < MAX_WALLS) ? _oldid + 1 : 0;
            }
        }
    }

    //壁の初期化
    void InitSpawner()
    {
        _prebPoint = START_OFFSET + _spawnInterval;
        for (int i = 0; i < MAX_WALLS; i++)
        {
            _walls[i].transform.localPosition = new Vector3(0f, 0f, _spawnInterval * (i + 1) + START_OFFSET);
            _walls[i].GetComponent<WallController>().SetWall();
        }
        _oldid = 0;
    }
}
