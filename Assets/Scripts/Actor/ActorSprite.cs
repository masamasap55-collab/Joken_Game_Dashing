using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// アクターのスプライトを設定するクラス (スプライト==表示画像の描画)
/// </summary>
public class ActorSprite : MonoBehaviour
{
    private ActorController actorController; //アクター制御クラス
    private SpriteRenderer spriteRenderer; //アクターのspriteRenderer

    //画像素材参照
    public List<Sprite> walkAnimationRes; // 歩行アニメーション(装備別*コマ数) List<Sprite>は2D画像を格納する可変長の配列の型 walkAnimationResourse
    public List<Sprite> stuckSpriteRes; //スタンスプライト(装備別)

    //各種変数
    private float walkAnimationTime; // 歩行アニメーション経過時間
    private int walkAnimationFrame; // 歩行アニメーションの画像番号
    private Tween blinkTween; //点滅処理Tween
    public bool stuckMode; //スタン画像表示モード

    //定数定義
    private const int WalkAnimationNum = 3; //歩行アニメーション画像枚数
    private const float WalkAnimationSpan = 0.1f; // アニメーション１枚スライド切り替え時間

    //初期化関数(ActorControllerから呼び出し)
    public void Init(ActorController _actorController) //Initとして初期化を定義することで、他のscriptからこのscriptを初期化しつつ、ActorControllerを参照することができるようになる。
    {
        //参照取得
        actorController = _actorController;
        spriteRenderer = actorController.GetComponent<SpriteRenderer>();
    }

    // Update
    void Update()
    {
        //被撃破中なら終了
        if (actorController.isDefeat)
            return;

        //スタン画像表示モード中ならスタン画像を表示
        if (stuckMode)
        {
            spriteRenderer.sprite = stuckSpriteRes[0];
            return;
        }

        //歩行アニメーション時間を経過(横移動してるときのみ計算)
        if (Mathf.Abs(actorController.xSpeed) > 0.0f) //Mathf.Abs()で絶対値
            walkAnimationTime += Time.deltaTime; //経過秒数を加算
        else if (actorController.xSpeed == 0) //止まってるときは立ち絵0にする。
            walkAnimationFrame = 0;

        //歩行アニメーションコマ数を計算
            if (walkAnimationTime >= WalkAnimationSpan)
            {
                walkAnimationTime -= WalkAnimationSpan;
                //アニメーション画像を次に
                walkAnimationFrame++;
                //アニメーション画像の番号を超えたら一巡させる。
                if (walkAnimationFrame >= WalkAnimationNum)
                    walkAnimationFrame = 0;
            }
        //歩行アニメーションの画像を更新
        spriteRenderer.sprite = walkAnimationRes[walkAnimationFrame];
    }

    /// <summary>
    /// 点滅開始処理
    /// </summary>
    public void StartBlinking()
    {
        // DoTweenを使った点滅処理
        blinkTween = spriteRenderer.DOFade (0.0f, 0.15f) //1回分の再生時間：0.15秒
            .SetDelay(0.3f) //0.3秒遅延
            .SetEase(Ease.Linear) //　線形変化
            .SetLoops(-1, LoopType.Yoyo); //無限ループ再生(偶数回は逆再生)
    }
    /// <summary>
    /// 被撃破演出開始
    /// </summary>
    public void StartDefeatAnim ()
    {
        //被撃破スプライト表示
        spriteRenderer.sprite = stuckSpriteRes[0];
        //点滅演出終了
        if (blinkTween != null)
            blinkTween.Kill();
        //スプライト非表示化アニメーション(DOTween)
        spriteRenderer.DOFade (0.0f, 2.0f); //2.0秒かけてスプライト非透明度を0.0fにする
    }
    /// <summary>
    /// 点滅終了処理
    /// </summary>
    public void EndBlinking()
    {
        // DoTweenの点滅処理を終了させる
        if (blinkTween != null)
        {
            blinkTween.Kill(); //Tweenを終了
            spriteRenderer.color = Color.white; //色を元に戻す
            blinkTween = null; //Tween情報を初期化
        }
    }

}
