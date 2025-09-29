using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージマネージャークラス
/// </summary>
public class StageManager : MonoBehaviour
{
    [HideInInspector] public ActorController actorController; //アクター制限クラス
    [HideInInspector] public CameraController cameraController; //カメラ制御クラス

    [Header("初期エリアのAreaManager")]//このをつけるとinspectorのUIで見出しが出て分かりやすい
    public AreaManager initArea; //ステージ内の最初のエリア(初期エリア) これは参照セットする。
    [Header("ボス戦用BGMのAudioClip")]
    public AudioClip bossBGMClip;

    //　ステージ内の全エリアの配列(Startで取得)
    private AreaManager[] inStageAreas;

    void Start()
    {
        //参照取得
        actorController = GetComponentInChildren<ActorController>();
        cameraController = GetComponentInChildren<CameraController>();

        //ステージ内の全エリアを取得・初期化
        inStageAreas = GetComponentsInChildren<AreaManager>();
        foreach (var targetAreaManager in inStageAreas) //foreachはvar A in B で Bという配列から一つずつAに入れてループ処理をする。
            targetAreaManager.Init(this);  //targetAreaManager == instageAreas == AreaManagerコンポーネント。

        //初期エリアをアクティブ化(その他のエリアは全て無効化)
        initArea.ActiveArea();
    }

    /// <summary>
    /// ステージ内の全エリアを無効化する
    /// </summary>
    public void DeactivateAllAreas()
    {
        foreach (var targetAreaManager in inStageAreas)
            targetAreaManager.gameObject.SetActive(false);
    }
    /// <summary>
    /// ボス戦用BGMを再生する
    /// </summary>
    public void PlayBossBGM()
    {
        // BGMを変更する
        GetComponent<AudioSource>().clip = bossBGMClip; //セット
        GetComponent<AudioSource>().Play(); //再生
    }

    /// <summary>
    /// ステージクリア時処理
    /// </summary>
    public void StageClear()
    {
        //ステージクリア処理
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    public void GameOver()
    {
        //ゲームオーバー処理
    }



}
