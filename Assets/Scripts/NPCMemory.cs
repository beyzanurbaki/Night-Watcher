using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCMemory
{
    public string eventType;
    public float emotionalImpact;
    public float timestamp;
    public float decayRate;
    public List<string> tags;

    public NPCMemory(string type, float impact, List<string> memoryTags = null)
    {
        eventType = type;
        emotionalImpact = Mathf.Clamp(impact, -1f, 1f);
        timestamp = Time.time;
        decayRate = 0.01f;
        tags = memoryTags ?? new List<string>();
    }

    public float GetStrength()
    {
        float elapsed = Time.time - timestamp;
        float stability = 1f / decayRate;
        float retention = Mathf.Exp(-elapsed / stability);
        return emotionalImpact * retention;
    }

    public bool ShouldBeRemoved()
    {
        return Mathf.Abs(GetStrength()) < 0.05f;
    }
}