using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGravitySetter : MonoBehaviour
{
    [Header("重力 (x , y)")]
    [SerializeField] private Vector2 sceneGravity = new Vector2(0, -9.81f);

    void Awake()
    {
        Physics2D.gravity = sceneGravity;
    }
}
