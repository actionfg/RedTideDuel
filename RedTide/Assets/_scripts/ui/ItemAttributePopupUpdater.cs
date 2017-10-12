using System;
using System.Text.RegularExpressions;
using Mgl;
using UnityEngine;
using UnityEngine.UI;

public class ItemAttributePopupUpdater : MonoBehaviour
{
    public RectTransform Popup;
    public Text Name;
    public Text AttributeText;

    private int _height;
//    private int _posY;
    private int _index;

//    private DroppedItemAttribute _lastItemAttribute;

    // Use this for initialization
    void Start()
    {
    }

    public void Switch()
    {
//        if (_lastItemAttribute != null)
//        {
//            _lastItemAttribute.OnShow(false);
//        }
//        _lastItemAttribute = null;
    }

//    public void Show(DroppedItemAttribute droppedItemAttribute)
//    {
//        if (_lastItemAttribute != null)
//        {
//            return;
//        }
//        else
//        {
//            if (_lastItemAttribute != null)
//            {
//                _lastItemAttribute.OnShow(false);
//            }
//            _lastItemAttribute = droppedItemAttribute;
//            droppedItemAttribute.OnShow(true);
//        }
//
//        foreach (Transform child in Popup)
//        {
//            child.gameObject.SetActive(false);
//        }
//
//        _height = 30;
//        _index = 0;
//        Name.color = Color.white;
//
//        if (droppedItemAttribute.WeaponAttribute != null)
//        {
//            Name.color = EffectUtil.GetColor(droppedItemAttribute.WeaponAttribute.Quality);
//            EquipmentAttribute weaponAttribute = droppedItemAttribute.WeaponAttribute;
//            EnchantingObject weaponEnchanting = GameContext.DropManager.EnchantingManager.GetEnchanting(weaponAttribute.Enchanting);
//            string prefix = (weaponEnchanting == null ? "" : weaponEnchanting.EffectName);
//            ShowNameText(I18n.Instance.__(I18NManager.WEAPON), I18n.Instance.__(droppedItemAttribute.Name), prefix);
//            ShowAttributeText(I18n.Instance.__(I18NManager.LEVEL), weaponAttribute.Level, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.AttackPower.ToString()), weaponAttribute.BaseAttackPower, false,
//                false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.AttackSpeed.ToString()), weaponAttribute.BaseAttackSpeed * 100, false, true);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Armor.ToString()), weaponAttribute.BaseArmor, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Resist.ToString()), weaponAttribute.BaseResist, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Vitality.ToString()), weaponAttribute.BaseVitality, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Strength.ToString()), weaponAttribute.Strength, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Intelligence.ToString()), weaponAttribute.Intelligence, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Vitality.ToString()), weaponAttribute.Vitality, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Spirit.ToString()), weaponAttribute.Spirit, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.CriticalChance.ToString()), weaponAttribute.CriticalChance * 100,
//                false, true);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.CriticalDamage.ToString()), weaponAttribute.CriticalDamage * 100,
//                false, true);
//            ShowAttributeText(I18n.Instance.__(I18NManager.EXTRA) + I18n.Instance.__(PowerUpAttribute.AttackPower.ToString()),
//                weaponAttribute.AttackPowerExtra, false, false);
//            ShowAttributeText(I18n.Instance.__(I18NManager.EXTRA) + I18n.Instance.__(PowerUpAttribute.Armor.ToString()),
//                weaponAttribute.ArmorExtra, false, false);
//            ShowAttributeText(I18n.Instance.__(I18NManager.EXTRA) + I18n.Instance.__(PowerUpAttribute.Resist.ToString()),
//                weaponAttribute.ResistExtra, false, false);
//            if (weaponEnchanting)
//            {
//                ShowEnchantingText(weaponEnchanting.EffectName, weaponEnchanting.Description, weaponAttribute.EnchantingValue);
//            }
//        }
//        else if (droppedItemAttribute.EquipAttribute != null)
//        {
//            Name.color = EffectUtil.GetColor(droppedItemAttribute.EquipAttribute.Quality);
//            EquipmentAttribute equipAttribute = droppedItemAttribute.EquipAttribute;
//            EnchantingObject equipEnchanting = GameContext.DropManager.EnchantingManager.GetEnchanting(equipAttribute.Enchanting);
//            string prefix = (equipEnchanting == null ? "" : equipEnchanting.EffectName);
//            ShowNameText(I18n.Instance.__(droppedItemAttribute.EquipType.ToString()),
//                I18n.Instance.__(droppedItemAttribute.Name), prefix);
//            ShowAttributeText(I18n.Instance.__(I18NManager.LEVEL), equipAttribute.Level, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.AttackPower.ToString()), equipAttribute.BaseAttackPower, false,
//                false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.AttackSpeed.ToString()), equipAttribute.BaseAttackSpeed * 100, false, true);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Armor.ToString()), equipAttribute.BaseArmor, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Resist.ToString()), equipAttribute.BaseResist, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Vitality.ToString()), equipAttribute.BaseVitality, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Strength.ToString()), equipAttribute.Strength, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Intelligence.ToString()), equipAttribute.Intelligence, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Vitality.ToString()), equipAttribute.Vitality, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Spirit.ToString()), equipAttribute.Spirit, false, false);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.CriticalChance.ToString()), equipAttribute.CriticalChance * 100,
//                false, true);
//            ShowAttributeText(I18n.Instance.__(PowerUpAttribute.CriticalDamage.ToString()), equipAttribute.CriticalDamage * 100,
//                false, true);
//            ShowAttributeText(I18n.Instance.__(I18NManager.EXTRA) + I18n.Instance.__(PowerUpAttribute.AttackPower.ToString()),
//                equipAttribute.AttackPowerExtra, false, false);
//            ShowAttributeText(I18n.Instance.__(I18NManager.EXTRA) + I18n.Instance.__(PowerUpAttribute.Armor.ToString()),
//                equipAttribute.ArmorExtra, false, false);
//            ShowAttributeText(I18n.Instance.__(I18NManager.EXTRA) + I18n.Instance.__(PowerUpAttribute.Resist.ToString()),
//                equipAttribute.ResistExtra, false, false);
//            if (equipEnchanting)
//            {
//                ShowEnchantingText(equipEnchanting.EffectName, equipEnchanting.Description, equipAttribute.EnchantingValue);
//            }
//        }
//        else if (droppedItemAttribute.ExtraAttribute != null)
//        {
//            DroppedExtraAttribute extraAttribute = droppedItemAttribute.ExtraAttribute;
//            ShowNameText(I18n.Instance.__(I18NManager.EXTRA) + I18n.Instance.__(I18NManager.ATTRIBUTE),
//                I18n.Instance.__(droppedItemAttribute.Name));
//            ShowAttributeText(I18n.Instance.__(extraAttribute.PowerUpAttribute.ToString()), extraAttribute.Value, true,
//                false);
//        }
//        else if (droppedItemAttribute.RecoverItemAttribute != null)
//        {
//            RecoverItemAttribute recoverItemAttribute = droppedItemAttribute.RecoverItemAttribute;
//            ShowNameText(I18n.Instance.__(I18NManager.RECOVER), I18n.Instance.__(droppedItemAttribute.Name));
//            ShowAttributeText(I18n.Instance.__(recoverItemAttribute.RecoverAttribute.ToString()),
//                recoverItemAttribute.Value, true, true);
//        }
//        else if (droppedItemAttribute.SoulAttribute != null)
//        {
//            SoulAttribute soulAttribute = droppedItemAttribute.SoulAttribute;
//            ShowNameText(I18n.Instance.__(I18NManager.SOUL), I18n.Instance.__(droppedItemAttribute.Name));
//            ShowAttributeText(I18n.Instance.__(I18NManager.TALENT) + I18n.Instance.__(I18NManager.FRAGMENT),
//                soulAttribute.TalentFragments, false, false);
//        }
//        else if (droppedItemAttribute.GoldAttribute != null)
//        {
//            GoldAttribute goldAttribute = droppedItemAttribute.GoldAttribute;
//            ShowNameText(I18n.Instance.__(I18NManager.GOLD), I18n.Instance.__(droppedItemAttribute.Name));
//            ShowAttributeText(I18n.Instance.__(I18NManager.GOLD), goldAttribute.Gold, false, false);
//        }
//        else if (droppedItemAttribute.SkillAttribute != null)
//        {
//            ShowNameText(I18n.Instance.__(I18NManager.SKILL), I18n.Instance.__(droppedItemAttribute.Name));
//        }
//        else if (droppedItemAttribute.ConsumableAttribute != null)
//        {
//            ShowNameText(I18n.Instance.__(droppedItemAttribute.Name), I18n.Instance.__(I18NManager.CONSUMABLE));
//        }
//        else
//        {
//            ShowNameText(I18n.Instance.__(I18NManager.ITEM), I18n.Instance.__(droppedItemAttribute.Name));
//        }
//        if (droppedItemAttribute.Price > 0)
//        {
//            // 显示价格
//            ShowAttributeText(I18n.Instance.__(I18NManager.PRICE), droppedItemAttribute.Price, false, false);
//        }
//        if (!String.IsNullOrEmpty(droppedItemAttribute.Description))
//        {
//            ShowDescription(I18n.Instance.__(droppedItemAttribute.Description));
//        }
//
//        _height += _index * 20;
//        Popup.sizeDelta = new Vector2(Popup.sizeDelta.x, _height);
//    }

    private void ShowNameText(string type, string itemName)
    {
        ShowNameText(type, itemName, "");
    }

    private void ShowNameText(string type, string itemName, string prefix)
    {
        var typeText = Popup.GetChild(_index++).gameObject.GetComponent<Text>();
        typeText.gameObject.SetActive(true);
        typeText.text = type;
        var nameText = Popup.GetChild(_index++).gameObject.GetComponent<Text>();
        nameText.gameObject.SetActive(true);
        nameText.text = prefix + itemName;
    }

    private void ShowAttributeText(string attributeStr, float attributeValue, bool IsFloatingPoint, bool IsPercent)
    {
        if (attributeValue > 0)
        {
            var attributeText = Popup.GetChild(_index++).gameObject.GetComponent<Text>();
            attributeText.gameObject.SetActive(true);
            attributeText.text = attributeStr;
            var valueText = attributeText.transform.GetChild(0).gameObject.GetComponent<Text>();
            valueText.text = attributeValue.ToString();
            if (IsFloatingPoint)
            {
                valueText.text = attributeValue.ToString();
            }
            else
            {
                valueText.text = Math.Round(attributeValue).ToString();
            }
            if (IsPercent)
            {
                valueText.text += "%";
            }
        }
    }

    private void ShowDescription(string description)
    {
        var descriptionText = Popup.GetChild(12).gameObject.GetComponent<Text>();
        descriptionText.gameObject.SetActive(true);
        descriptionText.text = description;

        var rectTransform = descriptionText.gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(15, -15 - _index * 20);

        int descriptionHeight = (int) LayoutUtility.GetPreferredHeight(rectTransform);
        _height += descriptionHeight;
    }

    private void ShowEnchantingText(string enchantingName, string enchantingEffect, float value)
    {
        string result = value.ToString("#0.0");
        string description = enchantingEffect.Replace("#", result);
        
        var enchantingText = Popup.GetChild(13).gameObject.GetComponent<Text>();
        enchantingText.gameObject.SetActive(true);
        enchantingText.text = enchantingName;
        var valueText = enchantingText.transform.GetChild(0).gameObject.GetComponent<Text>();
        valueText.text = description;
        
        var rectTransform = enchantingText.gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(15, -15 - _index * 20);
        _index++;
    }
}