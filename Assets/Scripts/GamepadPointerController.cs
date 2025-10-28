using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class GamepadPointerController : MonoBehaviour
{
    public RectTransform pointer;
    public Canvas canvas;
    public float moveSpeed = 1000f;
    private Vector2 pointerPos;

    private UIButtonEffect currentHoverButton; // 現在ホバー中のボタン

    void Start()
    {
        pointerPos = pointer.anchoredPosition;
    }

    void Update()
    {
        // スティック入力
        Vector2 stickInput = Vector2.zero;
        if (Gamepad.current != null)
            stickInput = Gamepad.current.leftStick.ReadValue();

        // ポインタ移動
        pointerPos += stickInput * moveSpeed * Time.deltaTime;
        pointer.anchoredPosition = pointerPos;

        // 現在ポインタの下にあるUIを取得
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = pointer.position;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        UIButtonEffect newHoverButton = null;

        foreach (var result in results)
        {
            newHoverButton = result.gameObject.GetComponent<UIButtonEffect>();
            if (newHoverButton != null)
                break;
        }

        // ホバー切り替え処理
        if (newHoverButton != currentHoverButton)
        {
            if (currentHoverButton != null)
                currentHoverButton.SetHover(false);
            if (newHoverButton != null)
                newHoverButton.SetHover(true);

            currentHoverButton = newHoverButton;
        }

        // ✕ボタンでクリック
        if (Gamepad.current != null && Gamepad.current.crossButton.wasPressedThisFrame)
        {
            if (currentHoverButton != null)
            {
                currentHoverButton.ClickEffect();

                // もしボタン自体にonClickがあるなら実行
                Button btn = currentHoverButton.GetComponent<Button>();
                if (btn != null) btn.onClick.Invoke();
            }
        }
    }
}
