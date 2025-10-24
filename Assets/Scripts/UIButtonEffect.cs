using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("サウンド設定")]
    public AudioSource audioSource;
    public AudioClip hoverClip;
    public AudioClip clickClip;

    [Header("アニメーション設定")]
    public float hoverScale = 1.1f;     // ホバー時の拡大率
    public float clickScale = 0.9f;     // クリック時の縮小率
    public float animSpeed = 8f;        // スケールアニメ速度

    private Vector3 defaultScale;
    private Vector3 targetScale;
    private bool isHovering = false;

    void Start()
    {
        defaultScale = transform.localScale;
        targetScale = defaultScale;
    }

    void Update()
    {
        // スムーズにスケール補間
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        targetScale = defaultScale * hoverScale;
        if (audioSource && hoverClip)
            audioSource.PlayOneShot(hoverClip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        targetScale = defaultScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioSource && clickClip)
            audioSource.PlayOneShot(clickClip);
        // 一瞬小さくしてすぐ戻す
        StopAllCoroutines();
        StartCoroutine(ClickAnimation());
    }

    private System.Collections.IEnumerator ClickAnimation()
    {
        targetScale = defaultScale * clickScale;
        yield return new WaitForSeconds(0.1f);
        targetScale = isHovering ? defaultScale * hoverScale : defaultScale;
    }
}
