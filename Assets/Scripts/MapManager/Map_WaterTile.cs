using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マップタイル：水中
/// 触れたアクターを水中モードにする
/// </summary>
public class Map_WaterTile : MonoBehaviour
{
    //トリガー滞在時に呼出
    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        //触れたアクターを水中モードにする
        if (tag == "Actor")
        {
            collision.gameObject.GetComponent<ActorController>().SetWaterMode(true);
        }
    }

    //トリガー退出時に呼出
    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        //水中モード解除
        if (tag == "Actor")
        {
            collision.gameObject.GetComponent<ActorController>().SetWaterMode(false);
        }
    }
}
