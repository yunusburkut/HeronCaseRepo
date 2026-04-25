using UnityEngine;

public class WaterView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hiddenColor;

    public Color Color { get; private set; }

    public void Init(Color color)
    {
        Color = color;
        spriteRenderer.color = color;
    }

    public void SetHidden(bool hidden)
    {
        if (hidden)
        {
            spriteRenderer.color = hiddenColor;
        }
        else
        {
            spriteRenderer.color = Color;
        }
    }

    public void SetSortingOrder(int order)
    {
        spriteRenderer.sortingOrder = order;
    }
}
