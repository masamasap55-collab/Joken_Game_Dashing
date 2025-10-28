using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class StartManager : MonoBehaviour
{
    public TextMeshProUGUI readyText; // Ready! のText
    public TextMeshProUGUI goText;    // Go! のText

    private bool gameStarted = false;

    void Start()
    {
        // 最初はReady表示、Go非表示
        readyText.gameObject.SetActive(true);
        goText.gameObject.SetActive(false);

        // 時間停止
        Time.timeScale = 0f;
    }

    void Update()
    {
        // Spaceが押されたら開始
        if (!gameStarted && (Input.GetKeyDown(KeyCode.Space) || Input.GetButton("Jump")))
        {
            gameStarted = true;
            StartCoroutine(StartSequence());
        }
    }

    IEnumerator StartSequence()
    {
        // Readyを消してGoを表示
        readyText.gameObject.SetActive(false);
        goText.gameObject.SetActive(true);
        goText.alpha = 1f;

        // 「Go!」を1秒でフェードアウト
        float fadeDuration = 1f;
        float elapsed = 0f;

        Time.timeScale = 1f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            goText.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        goText.gameObject.SetActive(false);

        // ゲーム開始！
        
    }
}
