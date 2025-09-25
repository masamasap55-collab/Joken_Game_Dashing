using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorGroundSensor : MonoBehaviour
{
    //コンポーネント参照
    private ActorController actorCtrl; //同じオブジェクトに割り当てられているなら他scriptをコンポーネントとして参照できる。

    //接地判定用変数
    public bool isGround = false; //接地中はtrue
    void Start()
    {
        //コンポーネント参照取得
        actorCtrl = GetComponentInParent<ActorController>(); //InParentで親オブジェクトにあるコンポーネントを取得できる。
    }

    //BoxCollider2Dに関するメソッド {OnTriggerStay2D} と {OnTriggerExit2D}
    //トリガー(当たり判定範囲)に滞在時に実行
    private void OnTriggerStay2D(Collider2D collision) //Collider2D == Box Collider2Dコンポーネント
    {
        if (collision.tag == "Ground") //collision.tagはトリガー内にあたっているもののタグを参照してる。タグは各オブジェクトに名称設定可能。
        {
            isGround = true;//接地フラグon
        }
    }
    //トリガーから離れたときに実行  
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            isGround = false;//接地フラグoff
        }
    }
}
