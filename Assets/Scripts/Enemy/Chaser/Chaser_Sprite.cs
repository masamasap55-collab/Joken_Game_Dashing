using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser_Sprite : MonoBehaviour
{
    [Header("アニメーションで使うスプライト群")]
    public List<Sprite> moveAnimationRes;

    [Header("切り替え間隔（秒）")]
    public float animationSpan = 0.2f;

    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // スプライトが設定されていれば最初の画像を表示
        if (moveAnimationRes.Count > 0)
            spriteRenderer.sprite = moveAnimationRes[0];
    }

    void Update()
    {
        if (moveAnimationRes.Count == 0) return;

        timer += Time.deltaTime;

        // animationSpan 秒経過したら次のスプライトへ
        if (timer >= animationSpan)
        {
            timer = 0f;
            currentIndex = (currentIndex + 1) % moveAnimationRes.Count; // ループ
            spriteRenderer.sprite = moveAnimationRes[currentIndex];
        }
    }
}
