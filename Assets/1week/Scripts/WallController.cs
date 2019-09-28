using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    Transform _playerTransform;

    [SerializeField]
    GameObject _cubePrefab;
    [SerializeField]
    Vector3 _wallSize;
    //変形を開始する距離
    [SerializeField]
    float _firstDistance;
    //変形を終了する距離
    [SerializeField]
    float _secondDistance;

    #region Properties

    #endregion

    const int MAX_CUBES = 4;
    GameObject[] _cubes = new GameObject[MAX_CUBES];

    int _pattern;
    Vector2 _holePos;
    float[] ratio = new float[MAX_CUBES];

    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        for (int i = 0; i < MAX_CUBES; i++)
        {
            _cubes[i] = Instantiate(_cubePrefab, Vector3.zero, Quaternion.identity);
            _cubes[i].transform.parent = this.transform;
        }

        SetWall();
    }

    void FixedUpdate()
    {
        //プレイヤーとの距離から変形率を計算
        float t = Mathf.Clamp01(1 - (transform.position.z - _playerTransform.position.z - _secondDistance) / (_firstDistance - _secondDistance));
        if (t > 0) MoveWall(t);
    }

    public void SetWall()
    {
        _pattern = -1;

        //壁を初期位置にセット
        _cubes[0].transform.localPosition = new Vector3(-_wallSize.x / 2, 0f, 0f);
        _cubes[0].transform.localScale = new Vector3(0f, _wallSize.y, _wallSize.z);

        _cubes[1].transform.localPosition = new Vector3(_wallSize.x / 2, 0f, 0f);
        _cubes[1].transform.localScale = new Vector3(0f, _wallSize.y, _wallSize.z);

        _cubes[2].transform.localPosition = new Vector3(0f, -_wallSize.y / 2, 0f);
        _cubes[2].transform.localScale = new Vector3(_wallSize.x, 0f, _wallSize.z);

        _cubes[3].transform.localPosition = new Vector3(0f, _wallSize.y / 2, 0f);
        _cubes[3].transform.localScale = new Vector3(_wallSize.x, 0f, _wallSize.z);
    }

    void MoveWall(float t)
    {
        if (_pattern < 0)
        {
            _pattern = (GameManager.Instance.hardMode == 0) ? Random.Range(0, 10) : (GameManager.Instance.hardMode == 1) ? Random.Range(10, 16) : Random.Range(0, 16);
            _holePos = new Vector2(Random.Range(-4.5f, 4.5f - GameManager.Instance.holeSize), Random.Range(-2.5f, 2.5f - GameManager.Instance.holeSize));
        }

        //パターンごとに変形の仕方を変える
        switch (_pattern)
        {
            case 0:
            case 1:
                ratio[0] = Mathf.Clamp01(t * t);
                ratio[1] = Mathf.Clamp01(t * t);
                ratio[2] = Mathf.Clamp01(t * t);
                ratio[3] = Mathf.Clamp01(t * t);
                break;
            case 2:
                ratio[0] = Mathf.Clamp01(t * 2);
                ratio[1] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 2 - 1f);
                ratio[2] = Mathf.Clamp01(t * 2);
                ratio[3] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 2 - 1f);
                break;
            case 3:
                ratio[0] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 2 - 1f);
                ratio[1] = Mathf.Clamp01(t * 2);
                ratio[2] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 2 - 1f);
                ratio[3] = Mathf.Clamp01(t * 2);
                break;
            case 4:
                ratio[0] = Mathf.Clamp01(t * 2);
                ratio[1] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 2 - 1f);
                ratio[2] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 2 - 1f);
                ratio[3] = Mathf.Clamp01(t * 2);
                break;
            case 5:
                ratio[0] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 2 - 1f);
                ratio[1] = Mathf.Clamp01(t * 2);
                ratio[2] = Mathf.Clamp01(t * 2);
                ratio[3] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 2 - 1f);
                break;
            case 6:
            case 7:
                ratio[0] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 2 - 1f);
                ratio[1] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 2 - 1f);
                ratio[2] = Mathf.Clamp01(t * 2);
                ratio[3] = Mathf.Clamp01(t * 2);
                break;
            case 8:
            case 9:
                ratio[0] = Mathf.Clamp01(t * 2);
                ratio[1] = Mathf.Clamp01(t * 2);
                ratio[2] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 2 - 1f);
                ratio[3] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 2 - 1f);
                break;
            //ハード用
            case 10:
            case 11:
                ratio[0] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 4 - 2f);
                ratio[1] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 4 - 2f);
                ratio[2] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 4 - 2f);
                ratio[3] = (t < 0.5f) ? 0 : Mathf.Clamp01(t * 4 - 2f);
                break;
            case 12:
            case 13:
                ratio[0] = (t < 0.2f) ? Mathf.Clamp01(t * 10) : (t < 0.8f) ? 0 : 1;
                ratio[1] = (t < 0.6f) ? Mathf.Clamp01(t * 10 - 4f) : (t < 0.8f) ? 0 : 1;
                ratio[2] = Mathf.Clamp01(t * 10 - 6f);
                ratio[3] = (t < 0.4f) ? Mathf.Clamp01(t * 10 - 2f) : (t < 0.8f) ? 0 : 1;
                break;
            case 14:
            case 15:
                ratio[0] = (t < 0.35f) ? Mathf.Clamp01(t * 5) : (t < 0.8f) ? 0 : 1;
                ratio[1] = (t < 0.35f) ? Mathf.Clamp01(t * 5) : (t < 0.8f) ? 0 : 1;
                ratio[2] = (t < 0.35f) ? Mathf.Clamp01(t * 5) : (t < 0.8f) ? 0 : 1;
                ratio[3] = (t < 0.35f) ? Mathf.Clamp01(t * 5) : (t < 0.8f) ? 0 : 1;
                break;
        }

        //lerpで各辺を変形
        _cubes[0].transform.localPosition = new Vector3(Mathf.Lerp(-_wallSize.x / 2, (-_wallSize.x / 2 + _holePos.x) / 2, ratio[0]), 0f, 0f);
        _cubes[0].transform.localScale = new Vector3(Mathf.Lerp(0f, _wallSize.x / 2 + _holePos.x, ratio[0]), _wallSize.y, _wallSize.z);

        _cubes[1].transform.localPosition = new Vector3(Mathf.Lerp(_wallSize.x / 2, (_wallSize.x / 2 + (_holePos.x + GameManager.Instance.holeSize)) / 2, ratio[1]), 0f, 0f);
        _cubes[1].transform.localScale = new Vector3(Mathf.Lerp(0f, _wallSize.x / 2 - (_holePos.x + GameManager.Instance.holeSize), ratio[1]), _wallSize.y, _wallSize.z);

        _cubes[2].transform.localPosition = new Vector3(0f, Mathf.Lerp(-_wallSize.y / 2, (-_wallSize.y / 2 + _holePos.y) / 2, ratio[2]), 0f);
        _cubes[2].transform.localScale = new Vector3(_wallSize.x, Mathf.Lerp(0f, _wallSize.y / 2 + _holePos.y, ratio[2]), _wallSize.z);

        _cubes[3].transform.localPosition = new Vector3(0f, Mathf.Lerp(_wallSize.y / 2, (_wallSize.y / 2 + (_holePos.y + GameManager.Instance.holeSize)) / 2, ratio[3]), 0f);
        _cubes[3].transform.localScale = new Vector3(_wallSize.x, Mathf.Lerp(0f, _wallSize.y / 2 - (_holePos.y + GameManager.Instance.holeSize), ratio[3]), _wallSize.z);
    }
}
