using System;
using UnityEngine;

[System.Serializable]
public class Personality
{
    [Header("Big Five (OCEAN) - 0.0 ile 1.0 arasi")]
    [Range(0, 1)] public float openness = 0.5f;
    [Range(0, 1)] public float conscientiousness = 0.5f;
    [Range(0, 1)] public float extraversion = 0.5f;
    [Range(0, 1)] public float agreeableness = 0.5f;
    [Range(0, 1)] public float neuroticism = 0.5f;

    /// <summary>
    /// Kisillige gore unutma hizini hesaplar
    /// Nevrotik bireyler negatif anilari yavas unutur
    /// </summary>
    public float GetDecayRate(float emotionalImpact)
    {
        float baseDecay = 0.15f; //gecikme daha hýzlý olmasý için 0.01 den 0.1 e yükselttik

        if (emotionalImpact < 0)
        {
            // Negatif ani - yuksek nevrotiklik = yavas unutma
            return baseDecay * (1f - neuroticism * 0.8f);
        }
        else
        {
            // Pozitif ani - yuksek nevrotiklik = hizli unutma
            return baseDecay * (1f + neuroticism * 0.3f);
        }
    }
}