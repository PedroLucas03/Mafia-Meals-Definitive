using UnityEngine;

public class CreditsScroll : MonoBehaviour
{
    public float scrollSpeed = 50f; 
    public float destroyHeight = 10f; 

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;


        if (rectTransform.anchoredPosition.y > destroyHeight)
        {
            Destroy(gameObject);
        }
    }
}