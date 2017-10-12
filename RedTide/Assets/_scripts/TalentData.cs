using System;
using System.Collections.Generic;
using Lib.SimpleJSON;
using Mgl;
using UnityEngine;

[System.Serializable]
public class TalentData : IJsonSerializable
{
    public const string FileName = "td.gd";
    public static string SavePath
    {
        get { return Application.persistentDataPath + "/" + FileName; }
    }

    // 注意兑换成升级点数的比率
    private float _talentFragments;
    private float _talentFragmentsLastGame;
    public int TotalPoints { get; private set; }

    public Dictionary<TalentType, int> talentPoints = new Dictionary<TalentType, int>();

    public void UpgradeTalent(TalentType talentType)
    {
        // 检查已解锁的总点数是否达到talentType的Threshold
        if (TotalPoints >= GetUpgradeThreshold(talentType))
        {
            int point;
            talentPoints.TryGetValue(talentType, out point);
            // 检查当前TalentFragments是否能兑换成升级点
            int requireFragment = NextUpgradeFragments(talentType, point);
            if (_talentFragments >= requireFragment)
            {
                // 检查是否已升级满
                if (point < GetMaxPoints(talentType))
                {
                    talentPoints[talentType] = point + 1;
                    TotalPoints += 1;
                    ConsumeTalentFragments(requireFragment);
                }
            }
        }
    }

    //TODO 下一次升级所需灵魂碎片数
    public int NextUpgradeFragments(TalentType talentType, int currentPoint)
    {
        return 1;
    }

    public int GetPoint(TalentType talentType)
    {
        int point;
        talentPoints.TryGetValue(talentType, out point);
        return point;
    }

    // 由天赋点换算成实际游戏内数值
    public float GetActualGameValue(TalentType talentType)
    {
        int talentValue;
        talentPoints.TryGetValue(talentType, out talentValue);
        switch (talentType)
        {
            case TalentType.HP:
            case TalentType.MP:
            case TalentType.Armor:
            case TalentType.Resist:
            case TalentType.Damage:
                return talentValue * 1;
            case TalentType.MoveSpeed:
            case TalentType.AttackSpeed:
            case TalentType.CritRate:
                return talentValue * 0.015f;
            case TalentType.CritDamage:
                return talentValue * 0.05f;
            case TalentType.Leech:
                return talentValue * 0.005f;
            case TalentType.CDR:
                return talentValue * 0.03f;
            case TalentType.Combo:
                return talentValue * 0.01f;
            case TalentType.AutoRecover:
                return talentValue * 0.015f;
            case TalentType.TalentDrop:
                return talentValue * 0.05f;
            case TalentType.Heal:
                return talentValue * 0.05f;
            case TalentType.MpTransform:
                return talentValue * 0.03f;
            case TalentType.DamageReflect:
                return talentValue * 0.2f;
            case TalentType.GoldGain:
                return talentValue * 0.1f;
            case TalentType.GoldDrop:
                return talentValue * 0.08f;
            case TalentType.MerchantDiscount:
                return talentValue * 0.05f;
            case TalentType.HomeingSoul:
                return talentValue * 0.05f;
        }
        return talentValue;
    }

    // level start from 0
    public static int GetUpgradeThreshold(int level)
    {
        switch (level)
        {
            case 0:
                return 0;
            case 1:
                return 1;
            case 2:
                return 5;
            case 3:
                return 15;
            case 4:
                return 25;
            case 5:
                return 40;
            case 6:
                return 55;
            case 7:
                return 75;
            case 8:
                return 100;
            case 9:
                return 125;
            case 10:
                return 200;
            default:
                return 0;
        }
    }

    public static int GetUpgradeThreshold(TalentType type)
    {
        switch (type)
        {
            case TalentType.Demon1:
                return GetUpgradeThreshold(0);
            case TalentType.HP:
            case TalentType.Damage:
            case TalentType.Armor:
            case TalentType.Wolf:
                return GetUpgradeThreshold(1);
            case TalentType.MP:
            case TalentType.Resist:
            case TalentType.TalentDrop:
                return GetUpgradeThreshold(2);
            case TalentType.GoldGain:
            case TalentType.MpTransform:
            case TalentType.RandomWeapon:
                return GetUpgradeThreshold(3);
            case TalentType.MoveSpeed:
            case TalentType.TalentReserve:
            case TalentType.Frankenstein:
                return GetUpgradeThreshold(4);
            case TalentType.CDR:
            case TalentType.CritRate:
            case TalentType.GoldDrop:
            case TalentType.Leecher:
                return GetUpgradeThreshold(5);
            case TalentType.Heal:
            case TalentType.DamageReflect:
            case TalentType.AutoRecover:
            case TalentType.RandomItem:
            case TalentType.Demon2:
            case TalentType.Puppet:
            case TalentType.Skeleton:
                return GetUpgradeThreshold(6);
            case TalentType.AttackSpeed:
            case TalentType.Lich:
            case TalentType.HomeingSoul:
                return GetUpgradeThreshold(7);
            case TalentType.CritDamage:
            case TalentType.MerchantDiscount:
            case TalentType.Leech:
            case TalentType.Cowman:
                return GetUpgradeThreshold(8);
            case TalentType.Combo:
            case TalentType.Dragonman:
                return GetUpgradeThreshold(9);
            case TalentType.Majin:
            case TalentType.Demon3:
                return GetUpgradeThreshold(10);
            default:
                throw new ArgumentOutOfRangeException("TalentType", type, null);
        }
        return 0;
    }

    public static int GetMaxPoints(TalentType talentType)
    {
        switch (talentType)
        {
            case TalentType.Demon1:
            case TalentType.Demon2:
            case TalentType.Demon3:
            case TalentType.Majin:
                return 1;
            case TalentType.RandomWeapon:
            case TalentType.RandomItem:
                return 2;
            case TalentType.Wolf:
            case TalentType.Frankenstein:
            case TalentType.Leecher:
            case TalentType.Puppet:
            case TalentType.Skeleton:
            case TalentType.Lich:
            case TalentType.Cowman:
            case TalentType.Dragonman:
            case TalentType.TalentDrop:
            case TalentType.GoldGain:
            case TalentType.GoldDrop:
            case TalentType.Heal:
            case TalentType.MpTransform:
            case TalentType.MerchantDiscount:
                return 5;
            case TalentType.MoveSpeed:
            case TalentType.AttackSpeed:
            case TalentType.CritRate:
            case TalentType.CritDamage:
            case TalentType.Leech:
            case TalentType.DamageReflect:
            case TalentType.AutoRecover:
            case TalentType.CDR:
            case TalentType.Combo:
            case TalentType.TalentReserve:
                return 10;
            case TalentType.HomeingSoul:
                return 12;
            case TalentType.HP:
            case TalentType.MP:
            case TalentType.Armor:
            case TalentType.Resist:
            case TalentType.Damage:
                return 30;
            default:
                throw new ArgumentOutOfRangeException("TalentType", talentType, null);
        }
    }

    public static string GetTalentName(TalentType talentType)
    {
        switch (talentType)
        {
            case TalentType.Demon1:
                return I18n.Instance.__("Talent_Demon1");
            case TalentType.Demon2:
                return I18n.Instance.__("Talent_Demon2");
            case TalentType.Demon3:
                return I18n.Instance.__("Talent_Demon3");
            case TalentType.Wolf:
                return I18n.Instance.__("Talent_Wolf");
            case TalentType.Frankenstein:
                return I18n.Instance.__("Talent_Frankenstein");
            case TalentType.Leecher:
                return I18n.Instance.__("Talent_Leecher");
            case TalentType.Puppet:
                return I18n.Instance.__("Talent_Puppet");
            case TalentType.Skeleton:
                return I18n.Instance.__("Talent_Skeleton");
            case TalentType.Lich:
                return I18n.Instance.__("Talent_Lich");
            case TalentType.Cowman:
                return I18n.Instance.__("Talent_Cowman");
            case TalentType.Dragonman:
                return I18n.Instance.__("Talent_Dragonman");
            case TalentType.Majin:
                return I18n.Instance.__("Talent_Majin");
            case TalentType.HP:
                return I18n.Instance.__("Talent_HP");
            case TalentType.MP:
                return I18n.Instance.__("Talent_MP");
            case TalentType.Armor:
                return I18n.Instance.__("Talent_Armor");
            case TalentType.Resist:
                return I18n.Instance.__("Talent_Resist");
            case TalentType.MoveSpeed:
                return I18n.Instance.__("Talent_MoveSpeed");
            case TalentType.AttackSpeed:
                return I18n.Instance.__("Talent_AttackSpeed");
            case TalentType.Damage:
                return I18n.Instance.__("Talent_Damage");
            case TalentType.CritRate:
                return I18n.Instance.__("Talent_CritRate");
            case TalentType.CritDamage:
                return I18n.Instance.__("Talent_CritDamage");
            case TalentType.Leech:
                return I18n.Instance.__("Talent_Leech");
            case TalentType.DamageReflect:
                return I18n.Instance.__("Talent_DamageReflect");
            case TalentType.CDR:
                return I18n.Instance.__("Talent_CDR");
            case TalentType.Combo:
                return I18n.Instance.__("Talent_Combo");
            case TalentType.AutoRecover:
                return I18n.Instance.__("Talent_AutoRecover");
            case TalentType.Heal:
                return I18n.Instance.__("Talent_Heal");
            case TalentType.MpTransform:
                return I18n.Instance.__("Talent_MpTransform");
            case TalentType.GoldDrop:
                return I18n.Instance.__("Talent_GoldDrop");
            case TalentType.GoldGain:
                return I18n.Instance.__("Talent_GoldGain");
            case TalentType.MerchantDiscount:
                return I18n.Instance.__("Talent_MerchantDiscount");
            case TalentType.TalentDrop:
                return I18n.Instance.__("Talent_TalentDrop");
            case TalentType.TalentReserve:
                return I18n.Instance.__("Talent_TalentReserve");
            case TalentType.HomeingSoul:
                return I18n.Instance.__("Talent_HomeingSoul");
            case TalentType.RandomWeapon:
                return I18n.Instance.__("Talent_RandomWeapon");
            case TalentType.RandomItem:
                return I18n.Instance.__("Talent_RandomItem");
            default:
                throw new ArgumentOutOfRangeException("TalentType", talentType, null);
        }
    }

    public static string GetTalentDescription(TalentType talentType, int talentPoint)
    {
        string description = "";
        switch (talentType)
        {
            case TalentType.Demon1:
                description = I18n.Instance.__("Talent_Demon1_Desc");
                break;
            case TalentType.Demon2:
                description = I18n.Instance.__("Talent_Demon2_Desc");
                break;
            case TalentType.Demon3:
                description = I18n.Instance.__("Talent_Demon3_Desc");
                break;
            case TalentType.Wolf:
                description = I18n.Instance.__("Talent_Wolf_Desc" + talentPoint);
                break;
            case TalentType.Frankenstein:
                description = I18n.Instance.__("Talent_Frankenstein_Desc" + talentPoint);
                break;
            case TalentType.Leecher:
                description = I18n.Instance.__("Talent_Leecher_Desc" + talentPoint);
                break;
            case TalentType.Puppet:
                description = I18n.Instance.__("Talent_Puppet_Desc" + talentPoint);
                break;
            case TalentType.Skeleton:
                description = I18n.Instance.__("Talent_Skeleton_Desc" + talentPoint);
                break;
            case TalentType.Lich:
                description = I18n.Instance.__("Talent_Lich_Desc" + talentPoint);
                break;
            case TalentType.Cowman:
                description = I18n.Instance.__("Talent_Cowman_Desc" + talentPoint);
                break;
            case TalentType.Dragonman:
                description = I18n.Instance.__("Talent_Dragonman_Desc" + talentPoint);
                break;
            case TalentType.Majin:
                description = I18n.Instance.__("Talent_Majin_Desc");
                break;
            case TalentType.HP:
                description = I18n.Instance.__("Talent_HP_Desc");
                break;
            case TalentType.MP:
                description = I18n.Instance.__("Talent_MP_Desc");
                break;
            case TalentType.Armor:
                description = I18n.Instance.__("Talent_Armor_Desc");
                break;
            case TalentType.Resist:
                description = I18n.Instance.__("Talent_Resist_Desc");
                break;
            case TalentType.MoveSpeed:
                description = I18n.Instance.__("Talent_MoveSpeed_Desc");
                break;
            case TalentType.AttackSpeed:
                description = I18n.Instance.__("Talent_AttackSpeed_Desc");
                break;
            case TalentType.Damage:
                description = I18n.Instance.__("Talent_Damage_Desc");
                break;
            case TalentType.CritRate:
                description = I18n.Instance.__("Talent_CritRate_Desc");
                break;
            case TalentType.CritDamage:
                description = I18n.Instance.__("Talent_CritDamage_Desc");
                break;
            case TalentType.Leech:
                description = I18n.Instance.__("Talent_Leech_Desc");
                break;
            case TalentType.DamageReflect:
                description = I18n.Instance.__("Talent_DamageReflect_Desc");
                break;
            case TalentType.CDR:
                description = I18n.Instance.__("Talent_CDR_Desc");
                break;
            case TalentType.Combo:
                description = I18n.Instance.__("Talent_Combo_Desc");
                break;
            case TalentType.AutoRecover:
                description = I18n.Instance.__("Talent_AutoRecover_Desc");
                break;
            case TalentType.Heal:
                description = I18n.Instance.__("Talent_Heal_Desc");
                break;
            case TalentType.MpTransform:
                description = I18n.Instance.__("Talent_MpTransform_Desc");
                break;
            case TalentType.GoldDrop:
                description = I18n.Instance.__("Talent_GoldDrop_Desc");
                break;
            case TalentType.GoldGain:
                description = I18n.Instance.__("Talent_GoldGain_Desc");
                break;
            case TalentType.MerchantDiscount:
                description = I18n.Instance.__("Talent_MerchantDiscount_Desc");
                break;
            case TalentType.TalentDrop:
                description = I18n.Instance.__("Talent_TalentDrop_Desc");
                break;
            case TalentType.TalentReserve:
                description = I18n.Instance.__("Talent_TalentReserve_Desc");
                break;
            case TalentType.HomeingSoul:
                description = I18n.Instance.__("Talent_HomeingSoul_Desc");
                break;
            case TalentType.RandomWeapon:
                description = I18n.Instance.__("Talent_RandomWeapon_Desc");
                break;
            case TalentType.RandomItem:
                description = I18n.Instance.__("Talent_RandomItem_Desc");
                break;
            default:
                throw new ArgumentOutOfRangeException("TalentType", talentType, null);
        }

//        if (description.Contains("#"))
//        {
//            string talengValue = GameContext.talentData.GetActualGameValue(talentType).ToString();
//            description = description.Replace("#", talengValue);
//        }
//        else if (description.Contains("&"))   // percent
//        {
//            float talengValue = GameContext.talentData.GetActualGameValue(talentType) * 100f;
//            description = description.Replace("&", talengValue.ToString());
//        }
        return description;
    }

    public void ConsumeTalentFragments(float fragments)
    {
        _talentFragments -= fragments;
    }

    public float GetTalentFragments()
    {
        return _talentFragments;
    }

    public void ClearRemainingTalentFragments()
    {
        _talentFragments = 0;
    }

    public void SetLastGameFragments(float fragments)
    {
        _talentFragmentsLastGame = fragments;
        _talentFragments = fragments;
    }

    public float GetTalentFragmentsLastGame()
    {
        return _talentFragmentsLastGame;
    }

    public JSONNode SaveToJSON()
    {
        var node = new JSONClass();
        node.Add("_talentFragments", new JSONData(_talentFragments));
        node.Add("_talentFragmentsLastGame", new JSONData(_talentFragmentsLastGame));
        node.Add("TotalPoints", new JSONData(TotalPoints));
        foreach (var talentPoint in talentPoints)
        {
            node[talentPoint.Key.ToString()] = new JSONData(talentPoint.Value);
        }

        return node;
    }

    public void ReadFromJSON(JSONNode node)
    {
        var subNode = node["_talentFragments"];
        _talentFragments = 0;
        _talentFragmentsLastGame = 0;
        TotalPoints = 0;

        if (subNode != null)
        {
            _talentFragments = subNode.AsFloat;
        }
        subNode = node["_talentFragmentsLastGame"];
        if (subNode != null)
        {
            _talentFragmentsLastGame = subNode.AsFloat;
        }
        subNode = node["TotalPoints"];
        if (subNode != null)
        {
            TotalPoints = subNode.AsInt;
        }
        foreach (TalentType type in Enum.GetValues(typeof(TalentType)))
        {
            var jsonNode = node[type.ToString()];
            if (!jsonNode.Equals(null))
            {
                talentPoints[type] = jsonNode.AsInt;
            }
        }
    }
}