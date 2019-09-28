using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    [SerializeField]
    Color[] _colorList;
    int _prebColor;
    bool _isBlack;

    void OnEnable()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
    }
    void OnDisable()
    {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
    }

    void Start()
    {
        LeanTween.alpha(this.gameObject, 0f, 0f);
    }

    void GameManager_GameStateChanged(GameState newState, GameState oldState)
    {
        if (newState == GameState.Play)
        {
            InitBackGround();
        }
    }

    void InitBackGround()
    {
        _isBlack = false;
        LeanTween.cancel(this.gameObject);
        _prebColor = Random.Range(0, _colorList.Length);
        LeanTween.color(this.gameObject, _colorList[Random.Range(0, _colorList.Length)], 0f);
        LeanTween.alpha(this.gameObject, 0f, 0f);
        LeanTween.alpha(this.gameObject, 1f, 4f);
    }

    public void ChangeRandomColor()
    {
        if (!_isBlack && GameManager.Instance.hardMode != 1 && _colorList.Length > 0)
        {
            int currentColor = Random.Range(0, _colorList.Length - 1);
            //同じ色にならないようにする
            if (currentColor >= _prebColor) currentColor++;

            LeanTween.color(this.gameObject, _colorList[currentColor], 0.0f);
            _prebColor = currentColor;
        }
        else
        {
            _isBlack = true;
            LeanTween.color(this.gameObject, Color.black, 0.0f);
        }

        if (GameManager.Instance.hardMode == 2)
            if (GameManager.Instance.gameState == GameState.Movie)
                LeanTween.alpha(this.gameObject, 0f, 1f);
            else
                LeanTween.alpha(this.gameObject, 1f - ((GameManager.Instance.currentStage - 35) / 50f), 0f);
    }

}
