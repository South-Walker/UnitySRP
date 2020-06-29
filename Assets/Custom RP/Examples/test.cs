using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    static MaterialPropertyBlock block;
    static int baseColorId = Shader.PropertyToID("_BaseColor");
    [SerializeField]
    Color baseColor = Color.white;
    //脚本被加载与inspector中值被修改时
    private void OnValidate()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }
        block.SetColor(baseColorId, baseColor);
        GetComponent<Renderer>().material.SetColor("_BaseColor", baseColor);
    }
    private void Awake()
    {
        OnValidate();
    }
}
