using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ステージセレクト画面管理クラス
/// </summary>
public class StageSelectManager : MonoBehaviour
{
    public AudioSource bgmSource;



    void Start()
    {
        bgmSource.loop = true;
    }

    void Update()
    {
        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    /// <summary>
    /// ステージ選択ボタン押下時処理
    /// </summary>
    /// <param name="stageID">該当ステージID</param>
    public void OnStageSelectButtonPressed(int stageID)
    {
        // シーン切り替え
        SceneManager.LoadScene(stageID + 1);
    }
}
