using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmic_CircularLift : MonoBehaviour
{
    // このLiftのRigidbosy2D
    private Rigidbody2D rigidbody2D;
    //アクターのTransform(アクターが乗った時に取得)
    private Transform actorTrs;
    private ActorGroundSensor actorGroundSensor;


    //設定項目
    [Header("移動時間(高い程時間がかかる)")]
    public float MovingTime;
    [Header("半径")]
    public float Radius;
    [Header("初期角度(0-360)")]
    public float InitialAngle;
    [Header("回転方向反転フラグ")]
    public bool Reverse;

    //各種変数
    private Vector3 defaultPos; //初期座標
    private Vector3 moveVec; //移動量ベクトル
    private float time; //経過時間
    private bool isActorRiding; //アクターが乗っている

    //Start
    void Start()
    {
        //参照取得
        rigidbody2D = GetComponent<Rigidbody2D>();

        //変数初期化
        defaultPos = transform.position;
        rigidbody2D.freezeRotation = true; //回転を無効化
        moveVec = Vector3.zero;
        time = 0.0f;

        //エラー回避
        if (MovingTime < 0.1f)
            MovingTime = 0.1f;

        //初期の移動進行率判定
        float fixValue = InitialAngle * Mathf.Deg2Rad;
        time = MovingTime * fixValue;
    }

    //FixedUpdate
    void FixedUpdate()
    {
        //時間経過
        time += Time.fixedDeltaTime;

        //移動ベクトル計算
        Vector3 vec;
        float moveValue = time / MovingTime;
        if (Reverse)
            moveValue *= -1.0f;
        vec = new Vector3(
            Mathf.Sin(moveValue) * Radius,
            Mathf.Cos(moveValue) * Radius,
            0.0f
        );

        //前フレームからの位置変化量を取得
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

    //トリガー滞在時に呼び出し
    private void OnTriggerStay2D(Collider2D collision)
    {
        //接しているのがアクターの接地判定オブジェクトでない場合は終了    
        var component = collision.gameObject.GetComponent<ActorGroundSensor>();
        if (component == null)
            return;

        actorGroundSensor = component;
        actorGroundSensor.isGround = true;
        //アクターのTransformを取得
        actorTrs = collision.gameObject.transform.parent;
        //アクターを移動させる処理の予約
        isActorRiding = true;
    }
}
