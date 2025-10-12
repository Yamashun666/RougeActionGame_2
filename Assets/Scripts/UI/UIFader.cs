using UnityEngine;
using DG.Tweening;

public class UIFader: MonoBehaviour
{
    public float motionWeight = 0.5f;
    public void UIFadeOut()
    {
        GetComponent<Renderer>().material.DOFade(0, motionWeight);
    }

    public void UIFadeIn()
    {
        GetComponent<Renderer>().material.DOFade(1, motionWeight);
    }
}
