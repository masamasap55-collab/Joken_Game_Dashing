using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 全エネミー共通処理クラス
/// </summary>
public class EnemyBase : MonoBehaviour
{
    //オブジェクト・コンポーネント
    [HideInInspector] public AreaManager areaManager; //エリアマネージャー
    protected Rigidbody2D rigidbody2D; //RigidBody2D
    protected SpriteRenderer spriteRenderer; //　敵スプライト
    protected Transform actorTransform; //主人公(アクター)のTransform

    //各種変数
    //基礎データ(インスペクタから入力)
    [Header("最大体力(初期体力)")] public int maxHP;
    [Header("接触時アクターへのダメージ")] public int touchDamage;
    //その他データ
    [HideInInspector] public int nowHP; //残りHP
    [HideInInspector] public bool isInvis; //無敵モード
    [HideInInspector] public bool rightFacing; //右向きフラグ(falseで左向き)

    // 初期化関数(AreaManager.csから呼出)
    public void Init(AreaManager _areaManager)
    {
        //参照取得
        areaManager = _areaManager;
        actorTransform = areaManager.stageManager.actorController.transform;
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //変数初期化    
        nowHP = maxHP;
        if (actorTransform.localScale.x > 0.0f)
            rightFacing = true;

        //エリアがアクティブになるまで何も処理せず待機
        gameObject.SetActive(false);
    }

    /// <summary>
    /// このモンスターの居るエリアにアクターが侵入した時の処理(エリアアクティブ化時処理)
    /// </summary>
    public virtual void OnAreaActivated()
    {
        //このモンスターをアクティブ化
        gameObject.SetActive(true);

    }

    /// <summary>
    /// ダメージを受ける際に呼び出される
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    /// <returns>ダメージ成功フラグ trueで成功</returns>
    public bool Damaged(int damage)
    {
        //　ダメージ処理
        nowHP -= damage;

        if (nowHP <= 0.0f)
        { //HP0の場合
            Vanish();
        }
        else
        { //まだHPが残っている場合

        }

        return true;
    }

    /// <summary>
    /// エネミーが消滅する際に呼び出される
    /// </summary>
    private void Vanish()
    {
        //オブジェクト消滅
        Destroy(gameObject);
    }

    /// <summary>
    /// アクターに接触ダメージを与える処理
    /// </summary>
    public void BodyAttack(GameObject actorObj)
    {
        //アクターのコンポーネントを取得
        ActorController actorCtrl = actorObj.GetComponent<ActorController>();
        if (actorCtrl == null)
            return;

        //アクターにダメージを与える予定
        actorCtrl.Damaged(touchDamage);
    }

    /// <summary>
    /// オブジェクトの向きを左右で決定する
    /// </summary>
    /// <param name="isRight">右向きフラグ</param>
    public void SetFacingRight(bool isRight)
    {
        if (!isRight)
        {//左向き
            //スプライトを通常の向きで表示
            spriteRenderer.flipX = false;
            //右向きフラグoff
            rightFacing = false;
        }
        else
        {//右向き
            //スプライトを左右反転した向きで表示
            spriteRenderer.flipX = true;
            //右向きフラグon
            rightFacing = true;
        }
    }
}
