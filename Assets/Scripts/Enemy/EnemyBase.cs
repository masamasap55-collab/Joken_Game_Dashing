using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    //画像素材
    public Sprite sprite_Defeat; // 被撃破スプライト

    //各種変数
    //基礎データ(インスペクタから入力)
    [Header("最大体力(初期体力)")] public int maxHP;
    [Header("接触時アクターへのダメージ")] public int touchDamage;
    [Header("ボス敵フラグ(ONでボス敵として扱う。1ステージに1体のみ)")] public bool isBoss;
    //その他データ
    [HideInInspector] public int nowHP; //残りHP
    [HideInInspector] public bool isVanishing = false; //消滅フラグ trueで消滅中
    [HideInInspector] public bool isInvis; //無敵モード
    [HideInInspector] public bool rightFacing; //右向きフラグ(falseで左向き)

    //DoTween用
    private Tween damageTween; //被ダメージ時演出Tween

    //定数定義
    private readonly Color COL_DEFAULT = new Color(1.0f, 1.0f, 1.0f, 1.0f); //通常
    private readonly Color COL_DAMAGED = new Color(1.0f, 0.1f, 0.1f, 1.0f); //被ダメージ時カラー
    private const float KNOCKBACK_X = 1.8f; //被ダメージ時ノックバック力(x方向)
    private const float KNOCKBACK_Y = 0.3f; //被ダメージ時ノックバック力(y方向)

    // 初期化関数(AreaManager.csから呼出)
    public void Init(AreaManager _areaManager)
    {
        //参照取得
        areaManager = _areaManager;
        actorTransform = areaManager.stageManager.actorController.transform;
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //変数初期化 
        rigidbody2D.freezeRotation = true;   
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
          //被ダメージTween初期化
            if (damageTween != null)
                damageTween.Kill();
            damageTween = null;

            //消滅中フラグをセット
            isVanishing = true;
            //消滅中は物理演算なし
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            //点滅後に消滅処理を呼び出す(DoTween使用)
            spriteRenderer.DOFade(0.0f, 0.15f) // 0.15秒*ループ回数分の再生時間
                .SetEase(Ease.Linear) //線形変化
                .SetLoops(7, LoopType.Yoyo) //7回点滅
                .OnComplete(Vanish); //再生が終わったらVanish()を呼び出す設定

            // 被撃破時スプライトがあれば表示
            if (sprite_Defeat != null)
                spriteRenderer.sprite = sprite_Defeat;

            // その他撃破時処理
            if (isBoss)
            { //ボス撃破

            }
            else
            { //ザコ撃破

            }
        }
        else
        { //まだHPが残っている場合
            // 被ダメージTween初期化
            if (damageTween != null)
                damageTween.Kill();
            damageTween = null;
            //被ダメージ演出再生
            // (一瞬だけスプライトを赤色に変更)
            if (!isInvis)
            {
                spriteRenderer.color = COL_DAMAGED; //赤色に変更
                damageTween = spriteRenderer.DOColor(COL_DEFAULT, 1.0f); //DoTweenで徐々に戻す
            }

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
        //自身が消滅中なら無効
        if (isVanishing)
            return;
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
