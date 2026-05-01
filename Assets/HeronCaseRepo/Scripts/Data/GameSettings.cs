using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameSettings", menuName = "WaterSort/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Water Layout")]
    [SerializeField, FormerlySerializedAs("waterSlotHeight")] private float _waterSlotHeight = 0.5f;
    [SerializeField, FormerlySerializedAs("waterYStackOffset")] private float _waterYStackOffset = 0.25f;
    [SerializeField, FormerlySerializedAs("waterRevealDuration")] private float _waterRevealDuration = 0.3f;
    [SerializeField] private float _waterSegmentOverlap = 0.05f;

    [Header("Tube Selection")]
    [SerializeField, FormerlySerializedAs("outlineColor")] private Color _outlineColor = new Color(1f, 0.85f, 0.3f, 1f);
    [SerializeField, FormerlySerializedAs("outlineThickness")] private float _outlineThickness = 0.1f;
    [SerializeField, FormerlySerializedAs("liftAmount")] private float _liftAmount = 0.3f;
    [SerializeField, FormerlySerializedAs("liftDuration")] private float _liftDuration = 0.2f;
    [SerializeField, FormerlySerializedAs("anticipationDip")] private float _anticipationDip = 0.15f;
    [SerializeField, FormerlySerializedAs("anticipationDuration")] private float _anticipationDuration = 0.06f;

    [Header("Pour Animation")]
    [SerializeField, FormerlySerializedAs("pourOffsetX")] private float _pourOffsetX = 0.8f;
    [SerializeField, FormerlySerializedAs("pourAngle")] private float _pourAngle = 120f;
    [SerializeField, FormerlySerializedAs("pourDuration")] private float _pourDuration = 0.25f;
    [SerializeField, FormerlySerializedAs("pourHeightOffset")] private float _pourHeightOffset = 0.5f;
    [SerializeField, FormerlySerializedAs("pourHoldDuration")] private float _pourHoldDuration = 0.4f;
    [SerializeField, FormerlySerializedAs("pourArcHeight")] private float _pourArcHeight = 0.8f;

    [Header("Shake Animation")]
    [SerializeField, FormerlySerializedAs("shakeMagnitude")] private float _shakeMagnitude = 0.08f;
    [SerializeField, FormerlySerializedAs("shakeDuration")] private float _shakeDuration = 0.04f;
    [SerializeField, FormerlySerializedAs("shakeDecay1")] private float _shakeDecay1 = 0.6f;
    [SerializeField, FormerlySerializedAs("shakeDecay2")] private float _shakeDecay2 = 0.4f;

    [Header("Solved Animation")]
    [SerializeField, FormerlySerializedAs("solvedSquashX")] private float _solvedSquashX = 1.15f;
    [SerializeField, FormerlySerializedAs("solvedSquashY")] private float _solvedSquashY = 0.85f;
    [SerializeField, FormerlySerializedAs("solvedSquashDuration")] private float _solvedSquashDuration = 0.08f;
    [SerializeField, FormerlySerializedAs("solvedBounceDuration")] private float _solvedBounceDuration = 0.5f;

    [Header("Enter Animation")]
    [SerializeField] private float _enterDuration = 0.6f;
    [SerializeField] private float _enterStaggerDelay = 0.08f;

    [Header("Game")]
    [SerializeField, FormerlySerializedAs("queuedPourSpeedMultiplier")] private float _queuedPourSpeedMultiplier = 1.75f;

    public float WaterSlotHeight
    {
        get
        {
            return _waterSlotHeight;
        }
    }

    public float WaterSegmentOverlap => _waterSegmentOverlap;

    public float WaterYStackOffset
    {
        get
        {
            return _waterYStackOffset;
        }
    }

    public float WaterRevealDuration
    {
        get
        {
            return _waterRevealDuration;
        }
    }

    public Color OutlineColor
    {
        get
        {
            return _outlineColor;
        }
    }

    public float OutlineThickness
    {
        get
        {
            return _outlineThickness;
        }
    }

    public float LiftAmount
    {
        get
        {
            return _liftAmount;
        }
    }

    public float LiftDuration
    {
        get
        {
            return _liftDuration;
        }
    }

    public float AnticipationDip
    {
        get
        {
            return _anticipationDip;
        }
    }

    public float AnticipationDuration
    {
        get
        {
            return _anticipationDuration;
        }
    }

    public float PourOffsetX
    {
        get
        {
            return _pourOffsetX;
        }
    }

    public float PourAngle
    {
        get
        {
            return _pourAngle;
        }
    }

    public float PourDuration
    {
        get
        {
            return _pourDuration;
        }
    }

    public float PourHeightOffset
    {
        get
        {
            return _pourHeightOffset;
        }
    }

    public float PourHoldDuration
    {
        get
        {
            return _pourHoldDuration;
        }
    }

    public float PourArcHeight
    {
        get
        {
            return _pourArcHeight;
        }
    }

    public float ShakeMagnitude
    {
        get
        {
            return _shakeMagnitude;
        }
    }

    public float ShakeDuration
    {
        get
        {
            return _shakeDuration;
        }
    }

    public float ShakeDecay1
    {
        get
        {
            return _shakeDecay1;
        }
    }

    public float ShakeDecay2
    {
        get
        {
            return _shakeDecay2;
        }
    }

    public float SolvedSquashX
    {
        get
        {
            return _solvedSquashX;
        }
    }

    public float SolvedSquashY
    {
        get
        {
            return _solvedSquashY;
        }
    }

    public float SolvedSquashDuration
    {
        get
        {
            return _solvedSquashDuration;
        }
    }

    public float SolvedBounceDuration
    {
        get
        {
            return _solvedBounceDuration;
        }
    }

    public float QueuedPourSpeedMultiplier
    {
        get
        {
            return _queuedPourSpeedMultiplier;
        }
    }

    public float EnterDuration
    {
        get
        {
            return _enterDuration;
        }
    }

    public float EnterStaggerDelay
    {
        get
        {
            return _enterStaggerDelay;
        }
    }
}
