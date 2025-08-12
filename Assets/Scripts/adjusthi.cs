using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class adjushi : MonoBehaviour
{
    public float padding = 20f; // Optional extra space
    public float speed = 10f;

    private RectTransform rectTransform;
    private float targetHeight;
    private float originalHeight;
    public float expandedHeight = 300f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        targetHeight = rectTransform.sizeDelta.y;
        originalHeight = padding;
    }

    void Update()
    {
        targetHeight = CalculateChildrenHeight();

        Vector2 size = rectTransform.sizeDelta;
        size.y = Mathf.Lerp(size.y, targetHeight, Time.deltaTime * speed);
        rectTransform.sizeDelta = size;
    }

    float CalculateChildrenHeight()
    {
        float totalHeight = 0f;

        foreach (RectTransform child in transform)
        {
            // Use preferred height if LayoutElement is present, otherwise sizeDelta
            LayoutElement layout = child.GetComponent<LayoutElement>();
            if (layout != null && layout.preferredHeight > 0)
            {
                totalHeight += layout.preferredHeight;
            }
            else
            {
                totalHeight += child.sizeDelta.y;
            }
        }

        return totalHeight;
    }
}
