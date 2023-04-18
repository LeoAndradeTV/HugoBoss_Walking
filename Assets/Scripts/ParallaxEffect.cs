using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public float speed = 1.0f;
    private RectTransform textRectTransform;

    void Start()
    {
        textRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        float newX = textRectTransform.anchoredPosition.x - (speed * Time.deltaTime);
        if (newX < -textRectTransform.rect.width)
        {
            newX += textRectTransform.rect.width *2;
        }
        textRectTransform.anchoredPosition = new Vector2(newX, textRectTransform.anchoredPosition.y);
    }
}
