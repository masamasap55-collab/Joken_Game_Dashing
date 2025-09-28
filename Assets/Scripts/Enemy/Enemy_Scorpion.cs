using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 個別敵クラス： Scorpion
/// 
/// 移動せず竜巻攻撃
/// </summary>
public class Enemy_Scorpion : EnemyBase
{
    //オブジェクト・コンポーネント
    public GameObject tornadoBulletPrefab; // 竜巻プレハブ

    //画像素材
    [SerializeField] private List<Sprite> spriteList = null; //攻撃アニメ

    //設定項目
    [Header("攻撃間隔")]
    public float attackInterval;

    //各種変数
    private float timeCount;
    private float nextActionTime;

    // Start
    void Start()
    {
        //変数初期化
        timeCount = -1.0f;
        nextActionTime = attackInterval;
    }

    // Update
    void Update()
    {
        //消滅中なら処理しない
        if (isVanishing)
            return;

        //アクターとの位置関係から向きを決定
        if (transform.position.x > actorTransform.position.x)
        {   //左向き
            SetFacingRight(false);
        }
        else
        {   //右向き
            SetFacingRight(true);
        }

        //時間経過
        timeCount += Time.deltaTime;
        // スプライト適用
        int animationFrame = (int)(timeCount * spriteList.Count / attackInterval);
        animationFrame = Mathf.Clamp(animationFrame, 0, spriteList.Count - 1);
        spriteRenderer.sprite = spriteList[animationFrame];
        // まだ行動タイミングでない場合は処理終了
        if (timeCount < attackInterval) //timeCountがattackIntervalを越えたら１回打つ
            return;
        //行動開始
        timeCount = 0.0f;

        //竜巻攻撃
        ShotBullet_Tornade();
    }

    /// <summary>
    /// 横に竜巻弾を射撃する処理
    /// </summary>
    private void ShotBullet_Tornade()
    {
        //弾丸オブジェクト生成・設定
        //射撃位置
        Vector2 startPos = transform.position;
        //全方位発射処理
        GameObject obj = Instantiate(tornadoBulletPrefab, startPos, Quaternion.identity);
        obj.GetComponent<EnemyShot>().Init(
            2.4f, //速度
            rightFacing ? 0 : 180, //角度
            3, //ダメージ量
            4.2f, //存在時間
            true //地面に当たると消える
        );
    }
}
