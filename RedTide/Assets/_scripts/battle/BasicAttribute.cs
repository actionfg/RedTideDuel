using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Reflection;
using Lib.SimpleJSON;

/**
 * Similar to Java @Interface attributes
 */
class AttributeConfig : Attribute
{
    internal AttributeConfig(AttributeMode mode)
    {
        this.Mode = mode;
    }

    public AttributeMode Mode { get; private set; }
}

/**
 * Extension Methods operating on AttributeType
 */
public static class AttributeExtension
{
    public static AttributeMode GetMode(this AttributeType type)
    {
        AttributeConfig config = GetAttr(type);
        return config.Mode;
    }

    private static AttributeConfig GetAttr(AttributeType p)
    {
        return (AttributeConfig) Attribute.GetCustomAttribute(ForValue(p), typeof(AttributeConfig));
    }

    private static MemberInfo ForValue(AttributeType p)
    {
        return typeof(AttributeType).GetField(Enum.GetName(typeof(AttributeType), p));
    }
}

public enum AttributeType
{
    [AttributeConfig(AttributeMode.Additive)] Mass,

    // HP Related
    // Max HP = ((CharacterBasicHP + CharacterHpPerVit * Vitality) + HpExtra) * (HpFactor)
    // MP同理
    [AttributeConfig(AttributeMode.Additive)] HpExtra,
    [AttributeConfig(AttributeMode.Additive)] HpFactor,
    [AttributeConfig(AttributeMode.Additive)] MpExtra,
    [AttributeConfig(AttributeMode.Additive)] MpFactor,

    // 力量，影响物理攻击
    [AttributeConfig(AttributeMode.Additive)] Strength,

    // 耐力，影响最大生命值与护甲
    [AttributeConfig(AttributeMode.Additive)] Vitality,

    // 智力，影响魔法攻击
    [AttributeConfig(AttributeMode.Additive)] Intelligence,

    // 精神，影响最大法力值与抗性
    [AttributeConfig(AttributeMode.Additive)] Spirit,

    // Critical
    [AttributeConfig(AttributeMode.Additive)] CriticalChance,
    [AttributeConfig(AttributeMode.Additive)] CriticalDamage,

    // Speed, Basis values
    [AttributeConfig(AttributeMode.Additive)] MoveSpeedExtra,
    [AttributeConfig(AttributeMode.Additive)] MoveSpeedFactor,

    [AttributeConfig(AttributeMode.Additive)] AttackSpeedFactor,

    // PhysicalDamage related
    [AttributeConfig(AttributeMode.Additive)] ArmorExtra,
    [AttributeConfig(AttributeMode.Additive)] ArmorFactor,
    [AttributeConfig(AttributeMode.Additive)] PenetrationFactor,

    [AttributeConfig(AttributeMode.Additive)] ResistExtra,
    [AttributeConfig(AttributeMode.Additive)] ResistFactor,

    [AttributeConfig(AttributeMode.Additive)] AttackPower,
    [AttributeConfig(AttributeMode.Additive)] SpellPower,

    [AttributeConfig(AttributeMode.Additive)] ReduceCDFactor,
    [AttributeConfig(AttributeMode.Additive)] BounceDamageFactor,
    [AttributeConfig(AttributeMode.Additive)] ComboFactor,

    [AttributeConfig(AttributeMode.Additive)] PhysicalLeech,
    [AttributeConfig(AttributeMode.Additive)] MagicLeech,
    [AttributeConfig(AttributeMode.Additive)] HpAutoRecover,
}

public enum NoManagedAttributeType
{
    CurrentHp,
    CurrentMp
}

public enum AttributeMode
{
    Additive,
    Multiply
}

[System.Serializable]
public class SimpleAttribute : ICloneable, IJsonSerializable
{
    // 基础属性
    Dictionary<int, float> basicFactors = new Dictionary<int, float>();
    // 额外属性
    Dictionary<int, float> factors = new Dictionary<int, float>();

    AttributeMode mode;
    float cachedValue;

    public float Value
    {
        get
        {
            lock (this)
            {
                if (changed)
                {
                    if (mode == AttributeMode.Additive)
                    {
                        cachedValue = 0f;
                    }
                    else
                    {
                        cachedValue = 1f;
                    }

                    foreach (KeyValuePair<int, float> factorEntry in factors)
                    {
                        if (mode == AttributeMode.Additive)
                        {
                            cachedValue += factorEntry.Value;
                        }
                        else
                        {
                            cachedValue *= factorEntry.Value;
                        }
                    }
                    foreach (KeyValuePair<int, float> basicEntry in basicFactors)
                    {
                        if (mode == AttributeMode.Additive)
                        {
                            cachedValue += basicEntry.Value;
                        }
                        else
                        {
                            cachedValue *= basicEntry.Value;
                        }
                    }

                    changed = false;
                }
                return cachedValue;
            }
        }
    }

    int nextHandle = 100;

    bool changed = true;

    public SimpleAttribute(AttributeMode mode)
    {
        this.mode = mode;
    }

    public int AddFactor(float factor, bool basic)
    {
        changed = true;
        int handle = Interlocked.Increment(ref nextHandle);
        if (basic)
        {
            basicFactors.Add(handle, factor);
        }
        else
        {
            factors.Add(handle, factor);
        }
        return handle;
    }

    public bool RemoveByHandle(int handle)
    {
        changed = true;
        if (factors.Remove(handle))
        {
            return true;
        }
        else
        {
            return basicFactors.Remove(handle);
        }
    }

    public float GetFactor(int handle)
    {
        float factor;
        if (!factors.TryGetValue(handle, out factor))
        {
            basicFactors.TryGetValue(handle, out factor);
        }
        return factor;
    }

    public void ClearFactors()
    {
        changed = true;
        factors.Clear();
    }

    public object Clone()
    {
        SimpleAttribute clone = new SimpleAttribute(mode);
        clone.basicFactors = new Dictionary<int, float>(basicFactors);
        clone.factors = new Dictionary<int, float>(factors);
        clone.cachedValue = cachedValue;
        clone.nextHandle = nextHandle;
        clone.changed = changed;
        return clone;
    }

    public JSONNode SaveToJSON()
    {
        var rootNode = new JSONClass();
        rootNode.Add("Mode", new JSONData((int) mode));
        rootNode.Add("nextHandle", new JSONData(nextHandle));

        // 存储basicFactors
        var basicFactorKeyNodes = new JSONArray();
        var basicFactorsNode = new JSONClass();
        foreach (var key in basicFactors.Keys)
        {
            var keyData = new JSONData(key);
            basicFactorKeyNodes.Add(keyData);
            basicFactorsNode.Add(keyData.Value, new JSONData(basicFactors[key]));
        }
        rootNode.Add("basicFactorKeys", basicFactorKeyNodes);
        rootNode.Add("basicFactors", basicFactorsNode);

        // 存储factor
        var factorKeyNodes = new JSONArray();
        var factorsNode = new JSONClass();
        foreach (var key in factors.Keys)
        {
            var keyData = new JSONData(key);
            factorKeyNodes.Add(keyData);
            factorsNode.Add(keyData.Value, new JSONData(factors[key]));
        }
        rootNode.Add("FactorKeys", factorKeyNodes);
        rootNode.Add("Factors", factorsNode);

        return rootNode;
    }

    public void ReadFromJSON(JSONNode node)
    {
        mode = (AttributeMode) node["Mode"].AsInt;
        changed = true;
        nextHandle = node["nextHandle"].AsInt;
        var basicFactorKeys = node["basicFactorKeys"];
        var basicFactorNodes = node["basicFactors"];
        foreach (JSONNode keyNode in basicFactorKeys.AsArray)
        {
            basicFactors.Add(keyNode.AsInt, basicFactorNodes[keyNode.Value].AsFloat);
        }

        var factorKeys = node["FactorKeys"];
        var factorNodes = node["Factors"];
        foreach (JSONNode keyNode in factorKeys.AsArray)
        {
            factors.Add(keyNode.AsInt, factorNodes[keyNode.Value].AsFloat);
        }
    }
}

[System.Serializable]
public class BasicAttribute : IJsonSerializable
{
    Dictionary<AttributeType, SimpleAttribute> attributes;


    public BasicAttribute()
    {
        attributes = new Dictionary<AttributeType, SimpleAttribute>();
        foreach (AttributeType type in Enum.GetValues(typeof(AttributeType)))
        {
            attributes.Add(type, new SimpleAttribute(type.GetMode()));
        }
    }

    public int AddAttributeFactor(AttributeType type, float factor)
    {
        return AddAttributeFactor(type, factor, false);
    }

    public int AddAttributeFactor(AttributeType type, float factor, bool basic)
    {
        return attributes[type].AddFactor(factor, basic);
    }

    public bool RemoveAttributeFactorByHandle(AttributeType type, int handle)
    {
        return attributes[type].RemoveByHandle(handle);
    }

    public float GetAttributeFactorByHandle(AttributeType type, int handle)
    {
        return attributes[type].GetFactor(handle);
    }

    public float GetValue(AttributeType type)
    {
        return attributes[type].Value;
    }

    public void Init(BasicAttributeConfig config)
    {
        AddAttributeFactor(AttributeType.CriticalDamage, 2f, true);
        AddAttributeFactor(AttributeType.HpFactor, 1f, true);
        AddAttributeFactor(AttributeType.MpFactor, 1f, true);
        AddAttributeFactor(AttributeType.MoveSpeedFactor, 1f, true);
        AddAttributeFactor(AttributeType.AttackSpeedFactor, 1f, true);
        AddAttributeFactor(AttributeType.ArmorFactor, 1f, true);
        AddAttributeFactor(AttributeType.ResistFactor, 1f, true);
        AddAttributeFactor(AttributeType.ReduceCDFactor, 1f, true);
        AddAttributeFactor(AttributeType.BounceDamageFactor, 1f, true);
        AddAttributeFactor(AttributeType.ComboFactor, 1f, true);
        AddAttributeFactor(AttributeType.PhysicalLeech, 0f, true);
        AddAttributeFactor(AttributeType.MagicLeech, 0f, true);
        AddAttributeFactor(AttributeType.HpAutoRecover, 0f, true);
        AddAttributeFactor(AttributeType.PenetrationFactor, 0f, true);
    }

    public void Resume(BasicAttribute resumeAttribute)
    {
        foreach (var attribute in resumeAttribute.attributes)
        {
            attributes[attribute.Key] = attribute.Value;
        }
    }

    public BasicAttribute GetBasicAttribute()
    {
        BasicAttribute baseAttribute = new BasicAttribute();
        baseAttribute.attributes.Clear();
        foreach (var attribute in attributes)
        {
            SimpleAttribute cloneSimpleAttribute = (SimpleAttribute) attribute.Value.Clone();
            cloneSimpleAttribute.ClearFactors();
            baseAttribute.attributes.Add(attribute.Key, cloneSimpleAttribute);
        }
        return baseAttribute;
    }

    public JSONNode SaveToJSON()
    {
        var node = new JSONClass();
        foreach (var attribute in attributes)
        {
            node[attribute.Key.ToString()] = attribute.Value.SaveToJSON();
        }
        return node;
    }

    public void ReadFromJSON(JSONNode node)
    {
        foreach (AttributeType type in Enum.GetValues(typeof(AttributeType)))
        {
            var jsonNode = node[type.ToString()];
            if (!jsonNode.Equals(null))
            {
                var simpleAttribute = new SimpleAttribute(type.GetMode());
                simpleAttribute.ReadFromJSON(jsonNode);
                attributes[type] = simpleAttribute;
            }
        }
    }
}