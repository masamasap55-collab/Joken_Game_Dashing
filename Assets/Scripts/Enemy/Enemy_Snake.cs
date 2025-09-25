using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 個別敵クラス：Snake
/// 
/// アクターが近くにいると接近する
/// 攻撃はしてこないが体当たりしてくる
/// </summary>
public class Enemy_Snake : EnemyBase
{
    // Start
    void Start()
    {
    }

    /// <summary>
    /// このモンスターの居るエリアにアクターが侵入した時の起動時処理(エリアアクティブ化時処理)
    /// </summary>
    public override void OnAreaActivated()
    {
        //元々の起動時処理を実行
        base.OnAreaActivated();

        Debug.Log("追加の起動時処理");
    }

    // Update
    void Update()
    {
    }
}
