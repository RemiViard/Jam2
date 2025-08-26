using UnityEngine;
using UnityEngine.UI;
public class OxigenBarUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    float fillSpriteSize;
    [SerializeField] RectTransform fillTransform;
    [SerializeField] RectMask2D fillMask;
    Image fillImage;

    [SerializeField] Color panicColor;
    Color baseColor;

    bool isInPanic = false;
    float timerPanic = 0f;
    private void Start()
    {
        fillSpriteSize = fillTransform.rect.height;
        fillImage = fillTransform.GetComponent<Image>();
        baseColor = fillImage.color;
    }
    private void Update()
    {
        if (isInPanic)
        {
            fillImage.color = Color.Lerp(baseColor, panicColor, (Mathf.Cos(timerPanic)/2)+0.6f);
            timerPanic += Time.deltaTime * 10f;
            if (timerPanic > Mathf.PI * 2)
            {
                timerPanic = 0f;
            }
        }
    }
    public void OnValueChange(float value)
    {
        fillMask.padding = new Vector4(0,0,0, fillSpriteSize - (value * fillSpriteSize));
        if(value <= 0.3f && !isInPanic)
        {
            isInPanic = true;
            timerPanic = 0f;
        }
        else if(value > 0.3f && isInPanic)
        {
            isInPanic = false;
            fillImage.color = baseColor;
            timerPanic = 0f;
        }

    }
}
