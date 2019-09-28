using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdManager : MonoBehaviour
{
    struct Bird
    {
        public GameObject _gameObject;
        public BirdController _birdController;
        public Transform _transform;
        public Rigidbody _rigidbody;
    }

    Bird[] _birdArray;

    [SerializeField]
    int _maxBirdSize;

    [SerializeField]
    GameObject _target;
    Transform _targetTransform;
    Rigidbody _targetRigidbody;

    [SerializeField]
    Bounds _spawnField;
    [SerializeField]
    GameObject _birdPrefab;
    [SerializeField]
    float _xRotateScale;
    [SerializeField]
    float _zRotateScale;
    [Header("Boid Option")]
    [SerializeField]
    float _separationRange;
    [SerializeField]
    float _neighborRange;
    [Header("Factors")]
    [SerializeField]
    float _separationFactor;
    [SerializeField]
    float _alignmentFactor;
    [SerializeField]
    float _cohesionFactor;

    [SerializeField]
    int _birdCount;

    #region Properties
    public int birdCount
    {
        get { return _birdCount; }
        private set { _birdCount = value; }
    }
    #endregion

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
            InitBirds();
        }
    }

    void Start()
    {
        _targetTransform = _target.transform;
        _targetRigidbody = _target.GetComponent<Rigidbody>();

        _birdArray = new Bird[_maxBirdSize];

        for (int i = 0; i < _maxBirdSize; i++)
        {
            _birdArray[i]._gameObject = Instantiate(_birdPrefab, new Vector3(
                Random.Range(_spawnField.min.x, _spawnField.max.x),
                Random.Range(_spawnField.min.y, _spawnField.max.y),
                -110f
            ), Quaternion.identity);
            //構造体に参照を詰める
            _birdArray[i]._birdController = _birdArray[i]._gameObject.GetComponent<BirdController>();
            _birdArray[i]._transform = _birdArray[i]._gameObject.transform;
            _birdArray[i]._rigidbody = _birdArray[i]._gameObject.GetComponent<Rigidbody>();

            _birdArray[i]._transform.parent = this.transform;
            _birdArray[i]._birdController.birdState = BirdState.Boid;
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.gameState == GameState.Title || GameManager.Instance.gameState == GameState.Play || GameManager.Instance.gameState == GameState.Movie)
        {
            birdCount = BoidSimulate();
        }
    }

    void InitBirds()
    {
        for (int i = 0; i < _maxBirdSize; i++)
        {
            _birdArray[i]._transform.position = new Vector3(
                Random.Range(_spawnField.min.x, _spawnField.max.x),
                Random.Range(_spawnField.min.y, _spawnField.max.y),
                Random.Range(_spawnField.min.z, _spawnField.max.z));
            _birdArray[i]._rigidbody.velocity = Vector3.zero;
            _birdArray[i]._rigidbody.useGravity = false;
            _birdArray[i]._birdController.birdState = BirdState.Boid;
        }
        birdCount = _maxBirdSize;
    }

    int BoidSimulate()
    {
        int count = 0;
        for (int i = 0; i < _maxBirdSize; i++)
        {
            if (_birdArray[i]._birdController.birdState == BirdState.Boid)
            {
                _birdArray[i]._rigidbody.AddForce(Separation(_birdArray[i]) * _separationFactor + Alignment(_birdArray[i]) * _alignmentFactor
                    + Cohesion(_birdArray[i]) * _cohesionFactor, ForceMode.Acceleration);

                Vector3 vec = _birdArray[i]._rigidbody.velocity;
                vec.y *= (vec.y > 0) ? 0.8f : 1;

                if (vec == Vector3.zero)
                    _birdArray[i]._rigidbody.rotation *= Quaternion.Euler(vec.y * -_xRotateScale, 0f, vec.x * -_zRotateScale);
                else
                    _birdArray[i]._rigidbody.rotation = Quaternion.LookRotation(vec) * Quaternion.Euler(vec.y * -_xRotateScale, 0f, vec.x * -_zRotateScale);

                count++;
            }
        }
        return count;
    }

    //分離
    Vector3 Separation(Bird bird)
    {
        //XY平面上でターゲットと重なりにくくなるように
        Vector3 diff = bird._transform.position - new Vector3(_targetTransform.position.x, _targetTransform.position.y, bird._transform.position.z);
        Vector3 vec = (diff.magnitude < _separationRange) ? diff.normalized / diff.magnitude * diff.magnitude : Vector3.zero;
        int count = 1;

        foreach (var other in _birdArray)
        {
            if (other._birdController.birdState != BirdState.Boid || other._gameObject == bird._gameObject) continue;

            diff = bird._transform.position - other._transform.position;
            //一定距離以下なら反発する力を加える
            if (diff.magnitude < _separationRange)
            {
                vec += diff.normalized / diff.magnitude * diff.magnitude;
                count++;
            }
        }

        return (count > 0) ? vec / count : vec;
    }

    //連合
    Vector3 Alignment(Bird bird)
    {
        Vector3 vec = Vector3.zero;
        int count = 0;

        foreach (var other in _birdArray)
        {
            if (other._birdController.birdState != BirdState.Boid || other._gameObject == bird._gameObject) continue;

            if ((bird._transform.position - other._transform.position).magnitude < _neighborRange)
            {
                vec += other._rigidbody.velocity;
                count++;
            }
        }

        vec = (count > 0) ? vec / count : _targetRigidbody.velocity;
        vec = (vec + _targetRigidbody.velocity) / 2;
        vec.z = _targetRigidbody.velocity.z;

        return vec - bird._rigidbody.velocity;
    }

    //結合
    Vector3 Cohesion(Bird bird)
    {
        Vector3 vec = Vector3.zero;
        int count = 0;

        foreach (var other in _birdArray)
        {
            if (other._birdController.birdState != BirdState.Boid || other._gameObject == bird._gameObject) continue;

            if ((bird._transform.position - other._transform.position).magnitude < _neighborRange)
            {
                vec += other._transform.position;
                count++;
            }
        }

        vec = (count > 0) ? vec / count : _targetTransform.position;
        vec = (vec + _targetTransform.position) / 2;
        vec.z = _targetTransform.position.z;

        return vec - bird._transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_spawnField.center, _spawnField.size);
    }
}
