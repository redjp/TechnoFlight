using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSetup_Player : MonoBehaviour
{

    MaterialPropertyBlock _mpblock;
    void Start()
    {
        _mpblock = new MaterialPropertyBlock();
        _mpblock.SetColor("_Color", new Color(0.96f, 0.96f, 0.96f, 1f));
        GetComponent<MeshRenderer>().SetPropertyBlock(_mpblock);
    }
}
