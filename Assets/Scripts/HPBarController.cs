using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HPBarController : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;          // HPバーのUI
    [SerializeField] private ActorController actor;    // HPの参照元

    IEnumerator Start()
    {
    yield return null; // 1フレーム待つ
    hpSlider.maxValue = actor.maxHP;
    hpSlider.value = actor.nowHP;
    }

    void Update()
    {
        // 毎フレームHPを反映
        hpSlider.value = actor.nowHP;
    }
}
