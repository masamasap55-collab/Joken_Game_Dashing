using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージギミック：リフト
/// 上下または左右に往復運動を行う
/// </summary>
public class Gimmic_Lift : MonoBehaviour
{
    // このLiftのRigidbody2D
    private Rigidbody2D rigidbody2D;
    // アクターのTransform(アクターが乗った時に取得)
    private Transform actorTrs;
    private ActorGroundSensor actorGroundSensor;

    // 設定項目
    [Header("移動時間(高い程時間がかかる)")]
    public float MovingTime;
    [Header("移動距離")]
    public float MovingSpeed;
    [Header("移動方向(0-360)0で右、90で上")]
    public float MovingAngle;

    //各種変数
    private Vector3 defaultPos; //初期座標
    private Vector2 moveVec; //移動量ベクトル
    private float time; // 経過時間
    private bool isActorRiding; //アクターが乗っている

    // Start
    void Start()
    {
        //　参照取得
        rigidbody2D = GetComponent<Rigidbody2D>();

        // 変数初期化
        defaultPos = transform.position;
        rigidbody2D.freezeRotation = true; // 回転を無効化
        moveVec = Vector2.zero;
        time = 0.0f;

        //エラー回避
        if (MovingTime < 0.1f)
            MovingTime = 0.1f;
    }

    //FixedUpdate
    void FixedUpdate()
    {
        //時間経過
        time += Time.fixedDeltaTime;

        //移動ベクトル計算
        Vector3 vec;
        vec = new Vector3((Mathf.Sin(time / MovingTime) + 1.0f) * MovingSpeed, 0.0f, 0.0f);
        vec = Quaternion.Euler(0, 0, MovingAngle) * vec;

        //前フレームからの位置変更量を取得
        //(乗ったアクターを移動させる処理に使用する)
        moveVec = defaultPos + vec - transform.position;

        //移動適用
        rigidbody2D.MovePosition(defaultPos + vec);

        //アクターが乗っているならアクターを移動
        if (isActorRiding)
        {
            isActorRiding = false;
            actorTrs.Translate(moveVec);
            actorGroundSensor.isGround = true;
        }
    }

    //トリガー滞在時に呼出
    private void OnTriggerStay2D(Collider2D collision)
    {
        //　接しているのがアクターの接地判定オブジェクトでない場合は終了
        var componet = collision.gameObject.GetComponent<ActorGroundSensor>();
        if (componet == null)
            return;

        actorGroundSensor = componet;
        actorGroundSensor.isGround = true;
        //アクターのTransformを取得
        actorTrs = collision.gameObject.transform.parent;
        //アクターを移動させる処理の予約
        isActorRiding = true;

    }
}
