﻿using System;
 using System.Collections.Generic;
 using UnityEngine;

/**
 * The UnitManager of the Game
 *
 * Manages actual Unit creation & deletion
 * 用于区分不同阵营的怪物
 *
 */
public class UnitManager : MonoBehaviour
{
    public static readonly float ACTIVE_RANGE = 20f;
    public static readonly float Boss_ACTIVE_RANGE = 200f;
    public static UnitManager Instance;

    public GameObject MobHpPanel;

    public GameObject currentPlayer { get; private set; }

    // 用于保存一些Boss或场景的召唤物
    public List<DestroyableUnit> DestroyableUnits { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }

        DestroyableUnits = new List<DestroyableUnit>();
    }

    // Use this for initialization
	void Start () {
//	    if (currentPlayer == null)
//	    {
//	        PlayerConfigInfo configInfo = GameContext.ConfigInfo;
//	        GameContext.character = configInfo.CharacterIndex;
//	        PlayerConfig config = CharacterConfigManager.GetPlayer(configInfo.CharacterIndex);
//	        currentPlayer = config.Create();
//	        PlayerUnit playerUnit = currentUnit = currentPlayer.AddComponent<PlayerUnit>();
//	        playerUnit.Init(config, configInfo, GameContext.talentData);
//	        playerUnit.AddEffect(new UltraEnergeEffect(playerUnit));
//            Debug.Log("Creating Characeter: " + config.name + ", passive: " + configInfo.PassiveName + ", activeSkill: " + configInfo.ActiveSkill +
//                ", UltraSkill: " + configInfo.UltraSkill);
//	        Debug.Log("RandomGeneralPassive: " + configInfo.RandomGeneralPassive);
//
//	        if (GameContext.ResumeAttribute != null)
//	        {
//	            playerUnit.ResumeAttribute(GameContext.ResumeAttribute);
//	            GameContext.ResumeAttribute = null;
//	        }
//	        if (GameContext.ResumeWeaponId == 0)
//	        {
//                WeaponConfig weaponConfig = CharacterConfigManager.GetPlayerDefaultWeapon(configInfo.CharacterIndex);
//	            EquipmentAttribute weaponAttribute = new EquipmentAttribute(4, weaponConfig.WeaponQuality,
//                    weaponConfig.BaseAttributeFloatingRange / 100f, weaponConfig.ExtraAttributeFloatingRange / 100f, weaponConfig.ExtraAttributes, EquipmentAttributeType.Weapon);
//		        if (String.IsNullOrEmpty(GameContext.ConfigInfo.WeaponSkill))
//		        {
//			        weaponAttribute.Skill =
//				        GameContext.TalentUnlockManager.WeaponSkillManager.GetRandomWeaponSkill(weaponConfig.weaponType).name;
//		        }
//		        else
//		        {
//			        weaponAttribute.Skill = GameContext.ConfigInfo.WeaponSkill;
//		        }
//		        playerUnit.WeaponInstance = weaponConfig.CreateWeaponInstance(weaponAttribute);
//		        playerUnit.WeaponInstance.weaponSkill.Init(playerUnit);
//		        GameContext.ConfigInfo.WeaponSkill = weaponAttribute.Skill;
//		        
//		        EffectUtil.LoadExtraSavedData(playerUnit);
//	        }
//	        else    //读取存档
//	        {
//	            var weaponConfig = ItemCatalogueManager.Instance.GetItem(GameContext.ResumeWeaponId) as WeaponConfig;
//	            playerUnit.WeaponInstance  = weaponConfig.CreateWeaponInstance(GameContext.ResumeWeaponAttribute);
//		        playerUnit.WeaponInstance.weaponSkill.Init(playerUnit);
//		        EnchantingObject enchanting = GameContext.DropManager.EnchantingManager.GetEnchanting(GameContext.ResumeWeaponAttribute.Enchanting);
//		        if (enchanting)
//		        {
//			        EffectUtil.CreateEnchantingEffect(enchanting.gameObject, playerUnit.transform.position, playerUnit.transform.rotation, playerUnit,
//				        playerUnit.gameObject, playerUnit, EquipmentAttributeType.Weapon, GameContext.ResumeWeaponAttribute.EnchantingValue);
//		        }
//	            GameContext.ResumeWeaponId = 0;
//	            GameContext.ResumeWeaponAttribute = null;
//	        }
//	        if (GameContext.ResumeHelmId > 0)
//	        {
//	            var helmConfig = ItemCatalogueManager.Instance.GetItem(GameContext.ResumeHelmId) as EquipmentConfig;
//	            playerUnit.HelmInstance  = helmConfig.CreateEquipmentInstance(GameContext.ResumeHelmAttribute);
//		        EnchantingObject helmEnchanting = GameContext.DropManager.EnchantingManager.GetEnchanting(GameContext.ResumeHelmAttribute.Enchanting);
//		        if (helmEnchanting)
//		        {
//			        EffectUtil.CreateEnchantingEffect(helmEnchanting.gameObject, playerUnit.transform.position,
//				        playerUnit.transform.rotation, playerUnit, playerUnit.gameObject, playerUnit, EquipmentAttributeType.Helm, GameContext.ResumeHelmAttribute.EnchantingValue);
//		        }
//	            GameContext.ResumeHelmId = 0;
//	            GameContext.ResumeHelmAttribute = null;
//	        }
//	        if (GameContext.ResumeCloakId > 0)
//	        {
//	            var cloakConfig = ItemCatalogueManager.Instance.GetItem(GameContext.ResumeCloakId) as EquipmentConfig;
//	            playerUnit.CloakInstance  = cloakConfig.CreateEquipmentInstance(GameContext.ResumeCloakAttribute);
//		        EnchantingObject cloakEnchanting = GameContext.DropManager.EnchantingManager.GetEnchanting(GameContext.ResumeCloakAttribute.Enchanting);
//		        if (cloakEnchanting)
//		        {
//			        EffectUtil.CreateEnchantingEffect(cloakEnchanting.gameObject, playerUnit.transform.position,
//				        playerUnit.transform.rotation, playerUnit, playerUnit.gameObject, playerUnit, EquipmentAttributeType.Cloak, GameContext.ResumeCloakAttribute.EnchantingValue);
//		        }
//	            GameContext.ResumeCloakId = 0;
//	            GameContext.ResumeCloakAttribute = null;
//	        }
//		    if (GameContext.ResumeHp > 0)
//		    {
//				playerUnit.CurrentHp = GameContext.ResumeHp;
//				playerUnit.CurrentMp = GameContext.ResumeMp;
//		    }
//
//	        var playerControl = currentPlayer.AddComponent<PlayerControl>();
//	        currentPlayer.AddComponent<FloorHitControl>();
//	        currentPlayer.AddComponent<InteractiveControl>();
//	        currentPlayer.AddComponent<RetainOnLoad>();
//		    var droppedItemSearcher = currentPlayer.AddComponent<DroppedItemSearcher>();
//		    playerControl.DroppedItemSearcher = droppedItemSearcher;
//
//	        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
//	        CameraController controller = camera.GetComponent<CameraController>();
//            controller.Initialize(currentPlayer.transform);
//
//	       
////	        PetConfig petConfig = CharacterConfigManager.GetPet(0);
////	        GameObject pet = petConfig.Create();
////	        pet.transform.position = currentPlayer.transform.position + Vector3.back * 2f;
////	        pet.transform.rotation = currentPlayer.transform.rotation;
////	        currentPet = pet.AddComponent<PetUnit>();
////	        currentPet.Init(petConfig, GameContext.mobLevel);
////	        pet.AddComponent<PetAI2>();
//		    
//		    AssetGPULoader.Instance.PreloadPlayerSkillFx();
//	    }

	}

	// Update is called once per frame
	void Update () {
	}

    public void AddDestroyableUnit(DestroyableUnit unit)
    {
        DestroyableUnits.Add(unit);
    }
}
