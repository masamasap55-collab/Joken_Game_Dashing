using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特定のx座標を超えたら背景が徐々に変わる処理の制御クラス
/// </summary>
public class Change_BackGround : MonoBehaviour
{
    public SpriteRenderer backgroundSprite;

    [Header("変えるときのX座標")]
    [SerializeField] private int[] changex = new int[4];

    [Header("変える背景")]
    [SerializeField] private List<Sprite> backgroundRes;

    private int currentIndex = -1;
    private bool isChanging = false;

    void Update()
    {
        // 現在位置に応じてどの背景にすべきか判定
        int newIndex = -1;
        for (int i = changex.Length - 1; i >= 0; i--)
        {
            if (transform.position.x > changex[i])
            {
                newIndex = i;
                break;
            }
        }

        // 背景が変わる必要がある場合のみフェード開始
        if (newIndex != -1 && newIndex != currentIndex && !isChanging)
        {
            StartCoroutine(FadeToNextBackground(newIndex));
        }
    }

    private IEnumerator FadeToNextBackground(int newIndex)
    {
        isChanging = true;

        // 新しいスプライト用のSpriteRendererを一時的に生成
        GameObject fadeObj = new GameObject("FadeSprite");
        SpriteRenderer fadeRenderer = fadeObj.AddComponent<SpriteRenderer>();
        fadeRenderer.sprite = backgroundRes[newIndex];
        fadeRenderer.sortingOrder = backgroundSprite.sortingOrder + 1; // 前面に表示
        fadeRenderer.color = new Color(1f, 1f, 1f, 0f); // 透明から開始

        fadeObj.transform.SetParent(backgroundSprite.transform.parent);
        fadeObj.transform.position = backgroundSprite.transform.position;

        float duration = 0.6f;
        float elapsed = 0f;

        // 徐々にフェードイン
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            fadeRenderer.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        // 最後にSpriteを置き換えてフェードオブジェクトを削除
        backgroundSprite.sprite = backgroundRes[newIndex];
        Destroy(fadeObj);

        currentIndex = newIndex;
        isChanging = false;
    }
}
