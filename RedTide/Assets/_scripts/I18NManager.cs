using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I18NManager {

    private static readonly I18NManager instance = new I18NManager();

    public static I18NManager Instance
    {
        get
        {
            return instance;
        }
    }

    public static readonly string WEAPON = "Weapon";
    public static readonly string HELM = "Helm";
    public static readonly string Cloak = "Cloak";
    public static readonly string SWORD = "Sword";
    public static readonly string LEVEL = "Level";
    public static readonly string EXTRA = "Extra";
    public static readonly string ATTRIBUTE = "Attribute";
    public static readonly string RECOVER = "Recover";
    public static readonly string SOUL = "Soul";
    public static readonly string GOLD = "Gold";
    public static readonly string TALENT = "Talent";
    public static readonly string FRAGMENT = "Fragment";
    public static readonly string SKILL = "Skill";
    public static readonly string ITEM = "Item";
    public static readonly string PRICE = "Price";
    public static readonly string CONSUMABLE = "Consumable";
    public static readonly string PASSIVE_SKILL = "PassiveSkill";
    public static readonly string GENERAL_PASSIVE = "GeneralPassive";
    public static readonly string ACTIVE_SKILL = "ActiveSkill";
    public static readonly string WEAPON_SKILL = "WeaponSkill";
    public static readonly string GENERAL_SKILL = "GeneralSkill";
    public static readonly string ULTRA_SKILL = "UltraSkill";

}
