using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

[RequireComponent(typeof(Rigidbody))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    PlayerController _target;
    Transform _targetTransform;
    [SerializeField]
    float _yOffset;
    [SerializeField]
    float _zOffset;

    Rigidbody _rigidbody;
    Transform _transform;

    float _elapsedTime;

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
            InitCamera();
        }
        if (newState == GameState.Movie)
        {
            _rigidbody.AddForce(Vector3.forward * 100f, ForceMode.Impulse);
        }
    }
    void Start()
    {
        _targetTransform = _target.transform;
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }
    void FixedUpdate()
    {
        if (GameManager.Instance.gameState == GameState.Result)
        { }
        else if (_target.playerState == PlayerState.Die || GameManager.Instance.gameState == GameState.Movie)
        {
            _rigidbody.rotation = Quaternion.LookRotation(_targetTransform.position - _transform.position);
        }
        else
        {
            //プレイヤーを追跡
            var diff = _targetTransform.position - _transform.position;
            diff.y += _yOffset;
            diff.z += _zOffset;
            _rigidbody.AddForce(diff * (_target.fowardSpeed * 2 * GameManager.Instance.gameSpeed));
        }
    }

    //カメラ位置の初期化
    void InitCamera()
    {
        _transform.SetPositionAndRotation(new Vector3(0f, 0f, -2f), Quaternion.identity);
    }
}
