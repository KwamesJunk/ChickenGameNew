using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    Image bar;
    float fullLength;
    float thickness;

    // Start is called before the first frame update
    void Start()
    {
        bar = GetComponent<Image>();
        fullLength = bar.rectTransform.sizeDelta.x;
        thickness = bar.rectTransform.sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPercent(float percent)
    {
        bar.rectTransform.sizeDelta = new Vector2(percent / 100 * fullLength, 25);
    }

    public void Set(HitPoints hp)
    {
        bar.rectTransform.sizeDelta = new Vector2(100.0f * hp.Get() / hp.GetMax(), 25);
    }
}
