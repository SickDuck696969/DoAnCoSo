using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class adjustbox : MonoBehaviour
{
    public float padding = 20f;
    public float speed = 10f;

    private RectTransform rectTransform;
    private float targetWidth;
    private float originalWidth;
    public float expandedWidth = 300f;
    public float d = 30;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        targetWidth = rectTransform.sizeDelta.x;
        originalWidth = padding;
    }
    void Update()
    {
        targetWidth = CalculateChildrenWidth() + padding;

        Vector2 size = rectTransform.sizeDelta;
        size.x = Mathf.Lerp(size.x, targetWidth, Time.deltaTime * speed);
        rectTransform.sizeDelta = size;
    }

    float CalculateChildrenWidth()
    {
        float totalWidth = 0f;

        foreach (RectTransform child in transform)
        {

            LayoutElement layout = child.GetComponent<LayoutElement>();
            if (layout != null && layout.preferredWidth > 0)
            {
                totalWidth += layout.preferredWidth;
            }
            else
            {
                totalWidth += child.sizeDelta.x;
            }
        }

        return totalWidth;
    }
}
