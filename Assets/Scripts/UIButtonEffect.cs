using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class UIButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("サウンド設定")]
    public AudioSource audioSource;
    public AudioClip hoverClip;
    public AudioClip clickClip;

    [Header("アニメーション設定")]
    public float hoverScale = 1.1f;
    public float clickScale = 0.9f;
    public float animSpeed = 8f;

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
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHover(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickEffect();
    }

    // 🔽 外部からも呼び出せるように変更
    public void SetHover(bool hover)
    {
        if (hover == isHovering) return; // 同じ状態なら何もしない

        isHovering = hover;
        targetScale = hover ? defaultScale * hoverScale : defaultScale;

        if (hover && audioSource && hoverClip)
            audioSource.PlayOneShot(hoverClip);
    }

    public void ClickEffect()
    {
        if (audioSource && clickClip)
            audioSource.PlayOneShot(clickClip);
        StopAllCoroutines();
        StartCoroutine(ClickAnimation());
    }

    private IEnumerator ClickAnimation()
    {
        targetScale = defaultScale * clickScale;
        yield return new WaitForSeconds(0.1f);
        targetScale = isHovering ? defaultScale * hoverScale : defaultScale;
    }
}
