using UnityEngine;

public class WaterView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public Color Color { get; private set; }

    public void Init(Color color)
    {
        Color = color;
        spriteRenderer.color = color;
    }

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}