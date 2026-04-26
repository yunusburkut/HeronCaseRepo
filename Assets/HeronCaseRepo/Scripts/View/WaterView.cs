using DG.Tweening;
using UnityEngine;

public class WaterView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hiddenColor;

    private static readonly int RevealAmountId = Shader.PropertyToID("_RevealAmount");
    private MaterialPropertyBlock _mpb;
    private float _revealAmount = 1f;

    public TweenCallback CachedDestroySelf { get; private set; }

    public Color Color { get; private set; }

    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        CachedDestroySelf = DestroySelf;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

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

    public void SetReveal(float amount)
    {
        _revealAmount = amount;
        spriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(RevealAmountId, _revealAmount);
        spriteRenderer.SetPropertyBlock(_mpb);
    }

    // Appearing: OutCubic (fast in, soft settle). Disappearing: InCubic (slow start, fast exit)
    public Tween AnimateRevealTo(float to, float duration)
    {
        var ease = to > _revealAmount ? Ease.OutCubic : Ease.InCubic;
        return DOTween.To(() => _revealAmount, x => SetReveal(x), to, duration).SetEase(ease);
    }
}
