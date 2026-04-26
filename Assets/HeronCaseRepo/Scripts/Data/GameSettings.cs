using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "WaterSort/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Water Layout")]
    public float waterSlotHeight = 0.5f;
    public float waterYStackOffset = 0.25f;
    public float waterRevealDuration = 0.3f;

    [Header("Tube Selection")]
    public float liftAmount = 0.3f;
    public float liftDuration = 0.2f;
    public float anticipationDip = 0.15f;
    public float anticipationDuration = 0.06f;

    [Header("Pour Animation")]
    public float pourOffsetX = 0.8f;
    public float pourAngle = 120f;
    public float pourDuration = 0.25f;
    public float pourHeightOffset = 0.5f;
    public float pourHoldDuration = 0.4f;
    public float pourArcHeight = 0.8f;

    [Header("Shake Animation")]
    public float shakeMagnitude = 0.08f;
    public float shakeDuration = 0.04f;
    public float shakeDecay1 = 0.6f;
    public float shakeDecay2 = 0.4f;

    [Header("Solved Animation")]
    public float solvedSquashX = 1.15f;
    public float solvedSquashY = 0.85f;
    public float solvedSquashDuration = 0.08f;
    public float solvedBounceDuration = 0.5f;

    [Header("Game")]
    public float queuedPourSpeedMultiplier = 1.75f;
}
