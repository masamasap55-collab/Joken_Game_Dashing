using UnityEngine;

public class GameBGMController : MonoBehaviour
{
    public AudioSource bgmSource;   // BGM用AudioSource
    public ActorController player; // HPを参照するプレイヤー
    public Gimmic_Goal goalManager; // クリアや開始待機を参照するマネージャー

    void Start()
    {
        // BGMをループ設定
        bgmSource.loop = true;
        bgmSource.Stop(); // 最初は止めておく
    }

    void Update()
    {
        // 再生条件
        bool canPlay = player.nowHP > 0 && player.isGameStarting && !goalManager.isClearing;

        if (canPlay)
        {
            if (!bgmSource.isPlaying)
                bgmSource.Play(); // 再生開始
        }
        else
        {
            if (bgmSource.isPlaying)
                bgmSource.Stop(); // 停止
        }
    }
}
