using System;
using UnityEngine;

[System.Serializable]
public class Personality
{
    [Header("Big Five (OCEAN) - 0.0 ile 1.0 arasi")]

    [Tooltip("Deneyime Aciklik")]
    [Range(0, 1)] public float openness = 0.5f;

    [Tooltip("Sorumluluk")]
    [Range(0, 1)] public float conscientiousness = 0.5f;

    [Tooltip("Disadonukluk")]
    [Range(0, 1)] public float extraversion = 0.5f;

    [Tooltip("Uyumluluk")]
    [Range(0, 1)] public float agreeableness = 0.5f;

    [Tooltip("Nevrotiklik")]
    [Range(0, 1)] public float neuroticism = 0.5f;

    public float GetDecayRate(float emotionalImpact)
    {
        float baseDecay = 0.05f;

        if (emotionalImpact < 0)
        {
            return baseDecay * (1f - neuroticism * 0.8f);
        }
        else
        {
            return baseDecay * (1f + neuroticism * 0.3f);
        }
    }
}