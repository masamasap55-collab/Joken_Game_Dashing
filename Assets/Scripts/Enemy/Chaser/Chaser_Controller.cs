using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser_Controller : MonoBehaviour
{
    [Header("追いかける対象（Actor）")]
    [SerializeField] private Transform target;

    [Header("カメラ")]
    [SerializeField] private Camera mainCamera;

    [Header("カメラ中心からの最大距離")]
    [SerializeField] private float maxDistance = 10f;

    [Header("移動速度")]
    [SerializeField] private float speed = 3f;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (target == null) return;

        // 方向ベクトルを求める
        Vector3 direction = target.position - transform.position;

        // 正規化して速度をかける
        Vector3 move = direction.normalized * speed * Time.deltaTime;

        // 移動（物理を無視して直接座標を操作）
        transform.position += move;

        // 向きを変える（左右反転）
        if (move.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = move.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    // 🔹 ぶつかった時の処理（2D Collider 同士）
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // もし相手が "Actor" タグを持っていたら
        if (collision.gameObject.CompareTag("Actor"))
        {
            // Actorクラスのスクリプトを取得
            ActorController actor = collision.gameObject.GetComponent<ActorController>();

            // nullチェックして、ダメージ関数を呼ぶ
            if (actor != null)
            {
                actor.Damaged(999); // 例：1ダメージ与える
                Debug.Log("Actor damaged"); // Unity のログ出力を使用
            }
        }
    }

    void LateUpdate()
    {
        if (target == null || mainCamera == null) return;

        // --- 追尾処理 ---
        Vector3 direction = target.position - transform.position;
        Vector3 move = direction.normalized * speed * Time.deltaTime;
        transform.position += move;

        // --- カメラからの距離制限 ---
        Vector3 cameraCenter = mainCamera.transform.position;
        cameraCenter.z = transform.position.z; // 2DなのでZは合わせる

        Vector3 offset = transform.position - cameraCenter;
        float distance = offset.magnitude;

        if (distance > maxDistance)
        {
            // 境界（半径）に収める
            Vector3 clampedPosition = cameraCenter + offset.normalized * maxDistance;
            transform.position = clampedPosition;
        }

        // --- 向きの調整 ---
        if (move.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = move.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}
