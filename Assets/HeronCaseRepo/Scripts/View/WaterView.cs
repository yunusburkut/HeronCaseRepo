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
        spriteRenderer.color = hidden ? hiddenColor : Color;
    }
}