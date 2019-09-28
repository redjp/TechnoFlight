using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSetup_Birds : MonoBehaviour
{

    MaterialPropertyBlock _mpblock;
    void Start()
    {
        _mpblock = new MaterialPropertyBlock();
        //_mpblock.SetFloat("_FlapSpeed", Random.Range(14f, 20f));
        //_mpblock.SetFloat("_FlapOffset", Random.Range(0f, 2f * Mathf.PI));
        GetComponent<MeshRenderer>().SetPropertyBlock(_mpblock);
    }
}
