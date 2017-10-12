using System;
using Mgl;
using UnityEngine;
using UnityEngine.UI;

public class CurrentAttributePopupUpdater : MonoBehaviour
{
	public RectTransform Popup;
	public Text Name;
	public Text AttributeText;

	private int _height;
	private int _index;
	
//	private DroppedItemAttribute _lastItemAttribute;

	// Use this for initialization
	void Start()
	{
	}
	
	public void Switch()
	{
//		_lastItemAttribute = null;
	}

//	public void Show(DroppedItemAttribute droppedItemAttribute)
//	{
//		if (_lastItemAttribute != null)
//		{
//			return;
//		}
//		else
//		{
//			_lastItemAttribute = droppedItemAttribute;
//			// Is Equipment
//			bool active = IsActive(droppedItemAttribute);
//			gameObject.SetActive(active);
//			if (!active)
//			{
//				return;
//			}
//		}
//		
//		foreach (Transform child in Popup)
//		{
//			child.gameObject.SetActive(false);
//		}
//
//		_height = 30;
//		_index = 0;
//		Name.color = Color.white;
//
//		EquipmentAttribute equipAttribute = null;
//		string equipName = "";
//		string description = "";
//		if (droppedItemAttribute.WeaponAttribute != null)
//		{
//			equipAttribute = GameContext.UnitManager.currentUnit.WeaponInstance.WeaponAttribute;
//			equipName = GameContext.UnitManager.currentUnit.WeaponInstance.WeaponConfig.Name;
//			description = GameContext.UnitManager.currentUnit.WeaponInstance.WeaponConfig.Description;
//			Name.color = EffectUtil.GetColor(GameContext.UnitManager.currentUnit.WeaponInstance.WeaponAttribute.Quality);
//		}
//		else if (droppedItemAttribute.EquipAttribute != null)
//		{
//			if (droppedItemAttribute.EquipType == EquipmentType.Helm)
//			{
//				equipAttribute = GameContext.UnitManager.currentUnit.HelmInstance.EquipmentAttribute;
//				equipName = GameContext.UnitManager.currentUnit.HelmInstance.EquipmentConfig.Name;
//				description = GameContext.UnitManager.currentUnit.HelmInstance.EquipmentConfig.Description;
//				Name.color = EffectUtil.GetColor(GameContext.UnitManager.currentUnit.HelmInstance.EquipmentAttribute.Quality);
//			}
//			else if (droppedItemAttribute.EquipType == EquipmentType.Cloak)
//			{
//				equipAttribute = GameContext.UnitManager.currentUnit.CloakInstance.EquipmentAttribute;
//				equipName = GameContext.UnitManager.currentUnit.CloakInstance.EquipmentConfig.Name;
//				description = GameContext.UnitManager.currentUnit.CloakInstance.EquipmentConfig.Description;
//				Name.color = EffectUtil.GetColor(GameContext.UnitManager.currentUnit.CloakInstance.EquipmentAttribute.Quality);
//			}
//		}
//
//		if (equipAttribute != null)
//		{
//			EnchantingObject equipEnchanting = GameContext.DropManager.EnchantingManager.GetEnchanting(equipAttribute.Enchanting);
//			string prefix = (equipEnchanting == null ? "" : equipEnchanting.EffectName);
//			ShowNameText("当前装备", I18n.Instance.__(equipName), prefix);
//			ShowAttributeText(I18n.Instance.__(I18NManager.LEVEL), equipAttribute.Level, false, false);
//			ShowAttributeText(I18n.Instance.__(PowerUpAttribute.AttackPower.ToString()), equipAttribute.BaseAttackPower, false,
//				false);
//			ShowAttributeText(I18n.Instance.__(PowerUpAttribute.AttackSpeed.ToString()), equipAttribute.BaseAttackSpeed * 100, false, true);
//			ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Armor.ToString()), equipAttribute.BaseArmor, false, false);
//			ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Resist.ToString()), equipAttribute.BaseResist, false, false);
//			ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Vitality.ToString()), equipAttribute.BaseVitality, false, false);
//			ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Strength.ToString()), equipAttribute.Strength, false, false);
//			ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Intelligence.ToString()), equipAttribute.Intelligence, false, false);
//			ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Vitality.ToString()), equipAttribute.Vitality, false, false);
//			ShowAttributeText(I18n.Instance.__(PowerUpAttribute.Spirit.ToString()), equipAttribute.Spirit, false, false);
//			ShowAttributeText(I18n.Instance.__(PowerUpAttribute.CriticalChance.ToString()), equipAttribute.CriticalChance * 100,
//				false, true);
//			ShowAttributeText(I18n.Instance.__(PowerUpAttribute.CriticalDamage.ToString()), equipAttribute.CriticalDamage * 100,
//				false, true);
//			ShowAttributeText(I18n.Instance.__(I18NManager.EXTRA) + I18n.Instance.__(PowerUpAttribute.AttackPower.ToString()),
//				equipAttribute.AttackPowerExtra, false, false);
//			ShowAttributeText(I18n.Instance.__(I18NManager.EXTRA) + I18n.Instance.__(PowerUpAttribute.Armor.ToString()),
//				equipAttribute.ArmorExtra, false, false);
//			ShowAttributeText(I18n.Instance.__(I18NManager.EXTRA) + I18n.Instance.__(PowerUpAttribute.Resist.ToString()),
//				equipAttribute.ResistExtra, false, false);
//			if (equipEnchanting)
//			{
//				ShowEnchantingText(equipEnchanting.EffectName, equipEnchanting.Description, equipAttribute.EnchantingValue);
//			}
//		}
//		if (!String.IsNullOrEmpty(description))
//		{
//			ShowDescription(I18n.Instance.__(description));
//		}
//
//		_height += _index * 20;
//		Popup.sizeDelta = new Vector2(Popup.sizeDelta.x, _height);
//	}

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

//	private bool IsActive(DroppedItemAttribute droppedItemAttribute)
//	{
//		if (droppedItemAttribute.EquipAttribute == null && droppedItemAttribute.WeaponAttribute == null)
//		{
//			return false;
//		}
//		else if (droppedItemAttribute.EquipAttribute != null)
//		{
//			var playerUnit = GameContext.UnitManager.currentUnit;
//			var equipmentType = droppedItemAttribute.EquipType;
//			if (equipmentType == EquipmentType.Helm)
//			{
//				return playerUnit.HelmInstance != null;
//			}
//			else if (equipmentType == EquipmentType.Cloak)
//			{
//				return playerUnit.CloakInstance != null;
//			}
//		}
//		return true;
//	}
	
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