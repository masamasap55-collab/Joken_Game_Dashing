using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージギミック：ダメージ床
/// ブロックに乗ったアクター(またはトリガーに侵入したアクター)にダメージを与える
/// </summary>
public class Gimmic_DamageBlock : MonoBehaviour
{
    // 設定項目
    [Header("ダメージ量")]
    public int damage;

    //アクター接触時処理(Trigger)　こっちは枠内の判定 物理判定なしの時
    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Actor")
        {
            //アクターにダメージを与える
            collision.gameObject.GetComponent<ActorController>().Damaged(damage);
        }
    }
    //アクター接触時処理(Collider) こっちは衝突　物理判定ありの時
    private void OnCollisionStay2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Actor")
        {
            //アクターにダメージを与える
            collision.gameObject.GetComponent<ActorController>().Damaged(damage);
        }


    }
}
