using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// トリガー範囲内に侵入したアクターをスクロールさせる
/// </summary>
public class ScrollEnter : MonoBehaviour
{
    private CameraController cameraController;

    [Header("切り替え先エリアのAreaManager")] public AreaManager targetAreaManager; //参照セットで次のAreaを入れる。

    void Start()
    {
        // 参照取得
        cameraController = Camera.main.GetComponent<CameraController>();

        // Spriteを非表示にする
        GetComponent<SpriteRenderer>().enabled = false;
    }

    //各トリガー呼び出し処理
    //トリガー進入時に呼出
    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        //アクターが進入
        if (tag == "Actor")
        {
            targetAreaManager.ActiveArea();
        }
        
    }
}
