using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UIを扱うために必要
using UnityEngine.SceneManagement;
using TMPro;

public class Gimmic_Goal : MonoBehaviour
{
    private AudioSource audioSource;
    [Header("フェード用の白いImage")]
    [SerializeField] private Image fadeImage;

    [Header("ステージクリア表示用Text")]
    [SerializeField] private TextMeshProUGUI clearText;

    [Header("フェードにかける時間（秒）")]
    [SerializeField] private float fadeDuration = 0.6f;

    [Header("クリア音")]
    public AudioClip clearSound;

    public bool isClearing = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (fadeImage != null)
        {
            // 最初は透明にしておく
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        if (clearText != null)
            clearText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isClearing && collision.CompareTag("Actor"))
        {
            audioSource.PlayOneShot(clearSound);
            isClearing = true;
            StartCoroutine(FadeAndShowClear());
        }
    }

    private IEnumerator FadeAndShowClear()
    {
        float time = 0f;

        // フェード処理
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Clamp01(time / fadeDuration);

            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;
            }

            yield return null;
        }

        // ステージクリア文字を表示
        if (clearText != null)
        {
            clearText.gameObject.SetActive(true);
        }

        //ここでステージ遷移などを続けて呼ぶことも可能
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }
}
