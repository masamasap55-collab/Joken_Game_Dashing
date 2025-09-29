using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// ステージギミック：電気スイッチ
/// プレイヤー自身またはショットが触れると電気ドアを解除する
/// </summary>
public class Gimmic_ElectricalSwitch : MonoBehaviour
{
    //オブジェクト・コンポーネント
    private SpriteRenderer iconSpriteRenderer; //電気アイコンのSpriteRenderer

    //設定項目
    [Header("解除対象ドアオブジェクト")]
    public GameObject TargetDoor;
    [Header("オン状態の電気アイコン")]
    public Sprite iconSprite_On;

    //作動済みフラグ
    bool isActived;

    //Start
    void Start()
    {
        // 0番目の子オブジェクトが待つSpriteRendererへの参照を取得
        iconSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    //トリガー滞在時に呼出
    private void OnTriggerStay2D(Collider2D collision)
    {
        //すでに作動済みなら終了
        if (isActived)
            return;

        //接しているのがアクターまたはアクターのショットの時処理
        string tag = collision.gameObject.tag;
        if (tag == "Actor" || tag == "Actor_Shot")
        {
            //スイッチ作動
            isActived = true;
            iconSpriteRenderer.sprite = iconSprite_On;
            //解除対象ドア消去アニメ
            TargetDoor.transform.DOScale(0.0f, 1.0f)
                .OnComplete(() => Destroy(TargetDoor));
        }

    }




}
