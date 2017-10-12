using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Lib.SimpleJSON;
using UnityEngine;

[Serializable]
public class GoldData : IJsonSerializable
{
    public int Gold { get; private set; }
    private float _talentFragments;

    public void AddGold(int gold)
    {
        Gold += gold;
    }

    public void SpendGold(int gold)
    {
        if (Gold >= gold)
        {
            Gold -= gold;
        }
    }

    public void AddTalentFragments(float fragments)
    {
        _talentFragments += fragments;
    }

    public float GetTalentFragments()
    {
        return _talentFragments;
    }

    public JSONNode SaveToJSON()
    {
        var node = new JSONClass();
        node["Gold"] = new JSONData(Gold);
        node["talentFragments"] = new JSONData(_talentFragments);
        return node;
    }

    public void ReadFromJSON(JSONNode node)
    {
        Gold = node["Gold"].AsInt;
        _talentFragments = node["talentFragments"].AsFloat;
    }
}
