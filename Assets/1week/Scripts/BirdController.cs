using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BirdState
{
    Prepare,
    Boid,
    Die
}

public class BirdController : MonoBehaviour
{
    BirdState _birdState;

    Rigidbody _rigidbody;

    #region Properties
    public BirdState birdState
    {
        get { return _birdState; }
        set { _birdState = value; }
    }

    #endregion

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision other)
    {
        if (birdState == BirdState.Boid)
        {
            birdState = BirdState.Die;
            _rigidbody.useGravity = true;
        }
    }
}
