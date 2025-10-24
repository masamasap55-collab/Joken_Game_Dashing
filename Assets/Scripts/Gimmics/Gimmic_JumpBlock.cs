using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージギミック・ジャンプ台
/// </summary>
public class Gimmic_JumpBlock : MonoBehaviour
{
    private AudioSource audioSource;
    //設定項目
    [Header("ジャンプ力")]
    public float JumpPower;
    [Header("ジャンプ音")]
    public AudioClip jumpSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    //各トリガー呼び出し処理
    //トリガー滞在時に呼出
    private void OnTriggerStay2D(Collider2D collision)
    {
        //接しているのがアクターの接地判定オブジェクト出ない場合は終了
        ActorGroundSensor actorGroundSensor = collision.gameObject.GetComponent<ActorGroundSensor>();
        if (actorGroundSensor == null)
            return;

        //効果音
        audioSource.PlayOneShot(jumpSound);
        //アクターを移動させる
        var rigidbody2D = collision.gameObject.GetComponentInParent<Rigidbody2D>();
        rigidbody2D.velocity += new Vector2(0.0f, JumpPower);
    }
}
