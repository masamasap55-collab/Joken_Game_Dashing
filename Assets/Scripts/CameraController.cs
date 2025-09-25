using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// メインカメラ制御クラス(Main Cameraにアタッチ)
/// </summary>
public class CameraController : MonoBehaviour
{
    //オブジェクト・コンポーネント
    public Transform backGroundTransform; //背景スプライトのTransform(背景スプライトは参照セットする)

    //各種変数
    private Vector2 basePos; //基準座標
    private Rect limitQuad; //　有効中のカメラ移動制限範囲

    //定数定義
    public const float BG_Scroll_Speed = 0.5f; //背景スクロール速度(1.0fでカメラと同じ速度)

    /// <summary>
    /// カメラの位置を動かす
    /// </summary>
    /// <param name="targetPos">座標</param>
    public void SetPosition(Vector2 targetPos)
    {
        basePos = targetPos;
    }

    private void FixedUpdate()
    {
        //カメラ移動
        Vector3 pos = transform.localPosition; //現在の座標をposに代入 (カメラは３次元座標ベクトル)
        //　アクターの現在位置より少し右上を移すようにX・Y座標を修正
        pos.x = basePos.x + 2.5f; // X座標
        pos.y = basePos.y + 1.5f; // Y座標
                                  // Z座標は現在地(transform.localPosition)をそのまま使用

        //カメラ可動範囲を反映
        //Mathf.Clamp(宣言したい値, 許容の最小値, 許容の最大値)でカメラの動きを制限
        pos.x = Mathf.Clamp(pos.x, limitQuad.xMin, limitQuad.xMax); //xMin矩形の左端 xMax矩形の右端
        pos.y = Mathf.Clamp(pos.y, limitQuad.yMax, limitQuad.yMin); //yMax矩形の下端 yMin矩形の上端 (yMaxが最小値なのは矩形の場合y軸が下向きだから。)

        //計算後のカメラ座標を反映 Vector3.Lerpは少し遅れて反映される感じ (A, B, 線形補間率) AからBにスムーズに近づく感じ。 何％近づけた位置にするか　
        transform.localPosition = Vector3.Lerp(transform.localPosition, pos, 0.08f);

        //背景スプライト移動
        //(カメラが移動する半分の速度で移動する0.5fだから)
        Vector3 bgPos = transform.localPosition * BG_Scroll_Speed; //今のカメラの座標に0.5f
        bgPos.z = backGroundTransform.position.z; //z座標はそのまま
        backGroundTransform.position = bgPos; //カメラの座標の半分を代入
    }

    /// <summary>
    /// CameraMovingLimitter(カメラ移動範囲)を指定のものに切り替える
    /// </summary>
    /// <param name="targetMovingLimitter">切り替え先</param>
    public void ChangeMovingLimitter(CameraMovingLimitter targetMovingLimitter)
    {
        // カメラ可動制限範囲をセット
        limitQuad = targetMovingLimitter.GetSpriteRect(); //これでlimitQuadにはRectの情報が手に入る。
    }
}
