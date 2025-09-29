using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージギミック：隠し壁
/// アクターが通って欲しくない場所に設置する
/// </summary>
public class Gimmic_HiddenWall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().color = Color.clear;
    }

}
