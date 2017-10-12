using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RandomCharacterControl : MonoBehaviour
{
//    public CharacterConfigManager ConfigManager;
//    public Transform StageTransform;
//    public TalentUnlockConfigManager TalentUnlockConfigManager;
//    public GameObject StartView;
//    public GameObject ContinueView;
//    public GameObject RandomSkills1;
//    public GameObject RandomSkills2;
//    public Text GameMode;
//
//    private List<PlayerConfigInfo> _configInfos;
//    private int _currentIndex;
//    private int _lastIndex;
//    private SkillSwitchMotionControl _skillMotionControl1;
//    private SkillSwitchMotionControl _skillMotionControl2;
//
//    void Awake()
//    {
//        // 读取天赋
//        GameContext.LoadTalentData();
//
//        GameContext.LoadResumeData();
//        GameContext.LoadExtraSaveData();
//
//        GameMode.text = GameContext.Difficulty.ToString();
//
//        _skillMotionControl1 = RandomSkills1.GetComponent<SkillSwitchMotionControl>();
//        _skillMotionControl1.Current = true;
//        _skillMotionControl2 = RandomSkills2.GetComponent<SkillSwitchMotionControl>();
//        _skillMotionControl2.Current = false;
//    }
//
//    void Start()
//    {
//        
//        // 从persistantDataPath中读取预生成的三个随机角色(PlayerConfigInfo)
//        if (File.Exists(Application.persistentDataPath + "/rc.gd"))
//        {
//            var bf = new BinaryFormatter();
//            var fileStream = File.Open(Application.persistentDataPath + "/rc.gd", FileMode.Open);
//            _configInfos = (List<PlayerConfigInfo>) bf.Deserialize(fileStream);
//            fileStream.Close();
//        }
//        else
//        {        // 若没有则重新生成三个, 并保存
//            _configInfos = new List<PlayerConfigInfo>();
//            for (int i = 0; i < 3; i++)
//            {
//                _configInfos.Add(GetRandomConfigInfo(ConfigManager));
//            }
//            if (GameContext.ConfigInfo != null)
//            {
//                SaveConfigInfo();
//            }
//        }
//
//        if (SteamManager.Initialized)
//        {// 异步加载ResumeData
//            GameContext.OnResumeDataLoaded += OnLoadResumeData;
//        }
//        else
//        {
//            InsertSavedConfigInfo();
//        }
//        _currentIndex = 0;
//
//        RefreshStage();
//    }
//
//    private void InsertSavedConfigInfo()
//    {
//        if (GameContext.ConfigInfo != null)
//        {
//            // load saved character
//            SetupCharacter();
//        }
//    }
//
//    private void OnLoadResumeData(bool success)
//    {
//        if (success)
//        {
//            InsertSavedConfigInfo();
//
//            _currentIndex = 0;
//            RefreshStage();
//        }
//    }
//
//    // 初始化相应模型和界面
//    private void RefreshStage()
//    {
//        PlayerConfigInfo configInfo = _configInfos[_currentIndex];
//        PlayerConfigInfo lastConfigInfo = _configInfos[_lastIndex];
//        GameContext.ConfigInfo = configInfo;
//        for (int i = 0; i < StageTransform.childCount; i++)
//        {
//            var child = StageTransform.GetChild(i);
//            Destroy(child.gameObject);
//            var cloth = GameObject.FindObjectOfType<Cloth>();
//            if (cloth)
//            {
//                Destroy(cloth.transform.parent.gameObject);
//            }
//        }
//        PlayerConfig config = ConfigManager.GetPlayer(configInfo.CharacterIndex);
//        var characterObject = config.Create();
//        characterObject.transform.rotation = Quaternion.Euler(0, 180, 0);
//        characterObject.transform.parent = StageTransform;
//        // Set Equipments
//        var playerUnit = characterObject.AddComponent<PlayerUnit>();
//        playerUnit.Init(config, configInfo, GameContext.talentData);
//        if (configInfo.Saved)
//        {
//            var savedWeapon = ItemCatalogueManager.Instance.GetItem(GameContext.ResumeWeaponId) as WeaponConfig;
//            if (savedWeapon)
//            {
//                playerUnit.WeaponInstance = savedWeapon.CreateWeaponInstance(GameContext.ResumeWeaponAttribute);
//                playerUnit.WeaponInstance.weaponSkill.Init(playerUnit);
//            }
//
//            var savedHelm = ItemCatalogueManager.Instance.GetItem(GameContext.ResumeHelmId) as EquipmentConfig;
//            if (savedHelm)
//            {
//                playerUnit.HelmInstance = savedHelm.CreateEquipmentInstance(GameContext.ResumeHelmAttribute);
//            }
//
//            var savedCloack = ItemCatalogueManager.Instance.GetItem(GameContext.ResumeCloakId) as EquipmentConfig;
//            if (savedCloack)
//            {
//                playerUnit.CloakInstance = savedCloack.CreateEquipmentInstance(GameContext.ResumeCloakAttribute);
//            }
//        }
//        else
//        {
//            WeaponConfig weaponConfig = ConfigManager.GetPlayerDefaultWeapon(configInfo.CharacterIndex);
//            EquipmentAttribute weaponAttribute = new EquipmentAttribute(4, weaponConfig.WeaponQuality,
//                weaponConfig.BaseAttributeFloatingRange / 100f, weaponConfig.ExtraAttributeFloatingRange / 100f, weaponConfig.ExtraAttributes, EquipmentAttributeType.Weapon);
//            playerUnit.WeaponInstance = weaponConfig.CreateWeaponInstance(weaponAttribute);
//            if (playerUnit.WeaponInstance.weaponSkill)
//            {
//                playerUnit.WeaponInstance.weaponSkill.Init(playerUnit);
//            }
//            
//            EffectUtil.LoadExtraSavedData(playerUnit);
//        }
//
//        StartView.SetActive(!configInfo.Saved);
//        ContinueView.SetActive(configInfo.Saved);
//
//        var currentSkills = _skillMotionControl1.Current ? RandomSkills1 : RandomSkills2;
//        var nextSkills = _skillMotionControl1.Current ? RandomSkills2 : RandomSkills1;
//        SetupSkillShow(configInfo, config, nextSkills);
//        SetupSkillShow(lastConfigInfo, config, currentSkills);
//    }
//
//    private void SetupCharacter()
//    {
//        _configInfos.Insert(0, GameContext.ConfigInfo);
//    }
//
//    private void SaveConfigInfo()
//    {
//        var binaryFormatter = new BinaryFormatter();
//        var fileStream = File.Create(Application.persistentDataPath + "/rc.gd");
//        binaryFormatter.Serialize(fileStream, _configInfos);
//        fileStream.Close();
//    }
//
//    public void SwitchLeftCharacter()
//    {
//        _lastIndex = _currentIndex;
//        _currentIndex = (_currentIndex - 1) % _configInfos.Count;
//        if (_currentIndex < 0)
//        {
//            _currentIndex += _configInfos.Count;
//        }
//        RefreshStage();
//        
//        _skillMotionControl1.Switch(true);
//        _skillMotionControl2.Switch(true);
//    }
//
//    public void SwitchRightCharacter()
//    {
//        _lastIndex = _currentIndex;
//        _currentIndex = (_currentIndex + 1) % _configInfos.Count;
//        RefreshStage();
//
//        _skillMotionControl1.Switch(false);
//        _skillMotionControl2.Switch(false);
//    }
//
//    public void SwitchLastDifficulty()
//    {
//        var difficutyArray = Enum.GetValues(typeof(Difficulty));
//        int index = Math.Max((int) GameContext.Difficulty - 1, 0);
//        GameContext.Difficulty = (Difficulty) difficutyArray.GetValue(index);
//        GameMode.text = GameContext.Difficulty.ToString();
//    }
//
//    public void SwitchNextDifficulty()
//    {
//        var difficutyArray = Enum.GetValues(typeof(Difficulty));
//        int index = Math.Min((int) GameContext.Difficulty + 1, difficutyArray.Length - 1);
//        GameContext.Difficulty = (Difficulty) difficutyArray.GetValue(index);
//        GameMode.text = GameContext.Difficulty.ToString();
//    }
//
//    // 只在触发随机角色更新时(如游戏结束等)调用
//    public PlayerConfigInfo GetRandomConfigInfo(CharacterConfigManager configManager)
//    {
//        var playerConfigInfo = new PlayerConfigInfo();
////        playerConfigInfo.CharacterIndex = Random.Range(0, configManager.playerConfigs.Length);
//        PlayerConfig config = GetRandomPlayer(configManager);
//        playerConfigInfo.CharacterIndex = configManager.GetPlayerConfigIndex(config);
//        playerConfigInfo.ActiveSkill = GetRandomActiveSkill(config.activeSkills, null);
//        playerConfigInfo.UltraSkill = GetRandomUltraSkill(config.ultraSkills);
//        playerConfigInfo.PassiveName = GetRandomPassive(config.passives, null);
//        playerConfigInfo.RandomGeneralPassive = GetRandomGeneralPassive();
//        if (TalentUnlockConfigManager.IsExtraSkillUnlocked(config, true))
//        {
//            playerConfigInfo.ActiveSkill2 = GetRandomActiveSkill(config.activeSkills, playerConfigInfo.ActiveSkill);
//        }
//        if (TalentUnlockConfigManager.IsExtraSkillUnlocked(config, false))
//        {
//            playerConfigInfo.PassiveName2 = GetRandomPassive(config.passives, playerConfigInfo.PassiveName);
//        }
//        return playerConfigInfo;
//    }
//
//    public PlayerConfig GetRandomPlayer(CharacterConfigManager configManager)
//    {
//        List<PlayerConfig> unlockedPlayerConfigs = new List<PlayerConfig>();
//        for (int i = 0; i < configManager.GetPlayerConfigCount(); i++)
//        {
//            PlayerConfig playerConfig = configManager.GetPlayer(i);
//            if (TalentUnlockConfigManager.IsUnlocked(playerConfig))
//            {
//                unlockedPlayerConfigs.Add(playerConfig);
//            }
//        }
//
//        int randomIndex = Random.Range(0, unlockedPlayerConfigs.Count);
//        return unlockedPlayerConfigs[randomIndex];
//    }
//
//    private string GetRandomActiveSkill(SkillConfig[] activeSkills, string existed)
//    {
//        if (activeSkills.Length <= 0)
//        {
//            return "";
//        }
//        List<SkillConfig> unlockedSkills = new List<SkillConfig>();
//        foreach (var skill in activeSkills)
//        {
//            if (TalentUnlockConfigManager.IsUnlocked(skill) && !skill.name.Equals(existed))
//            {
//                unlockedSkills.Add(skill);
//            }
//        }
//        if (unlockedSkills.Count == 0)
//        {
//            return "";
//        }
//        else
//        {
//            return unlockedSkills[Random.Range(0, unlockedSkills.Count)].name;
//        }
//    }
//
//    private string GetRandomPassive(EffectObject[] passives, string existed)
//    {
//        if (passives.Length <= 0)
//        {
//            return "";
//        }
//        List<EffectObject> unlockedPassives = new List<EffectObject>();
//        foreach (var passive in passives)
//        {
//            if (TalentUnlockConfigManager.IsUnlocked(passive) && !passive.name.Equals(existed))
//            {
//                unlockedPassives.Add(passive);
//            }
//        }
//        if (unlockedPassives.Count == 0)
//        {
//            return "";
//        }
//        else
//        {
//            return unlockedPassives[Random.Range(0, unlockedPassives.Count)].name;
//        }
//    }
//
//    private string GetRandomUltraSkill(SkillConfig[] ultraSkills)
//    {
//        if (ultraSkills.Length == 0)
//        {
//            return "";
//        }
//
//        List<SkillConfig> unlockedSkills = new List<SkillConfig>();
//        foreach (var skill in ultraSkills)
//        {
//            if (TalentUnlockConfigManager.IsUnlocked(skill))
//            {
//                unlockedSkills.Add(skill);
//            }
//        }
//        if (unlockedSkills.Count == 0)
//        {
//            return "";
//        }
//        else
//        {
//            return unlockedSkills[Random.Range(0, unlockedSkills.Count)].name;
//        }
//    }
//
//    private string GetRandomGeneralPassive()
//    {
//        return GameContext.TalentUnlockManager.RandomPassiveManager.GetRandomPassive();
//    }
//
//    private void SetupSkillShow(PlayerConfigInfo configInfo, PlayerConfig config, GameObject randomSkills)
//    {
//        Image passiveIcon = randomSkills.transform.GetChild(1).gameObject.GetComponent<Image>();
//        var uiRandomSkillControl = passiveIcon.GetComponent<UIRandomSkillControl>();
//        EffectObject passive = GetPassive(config.passives, configInfo.PassiveName);
//        if (passive)
//        {
//            uiRandomSkillControl.RandomSkill = passive;
//            if (passive.Icon)
//            {
//                passiveIcon.sprite = passive.Icon;
//            }
//        }
//
//        Image generalPassiveIcon = randomSkills.transform.GetChild(0).gameObject.GetComponent<Image>();
//        var uiGeneralPassiveControl = generalPassiveIcon.GetComponent<UIRandomSkillControl>();
//        EffectObject generalPassive = GameContext.TalentUnlockManager.RandomPassiveManager.GetPassive(configInfo.RandomGeneralPassive);
//        if (generalPassive)
//        {
//            uiGeneralPassiveControl.RandomSkill = generalPassive;
//            if (generalPassive.Icon)
//            {
//                generalPassiveIcon.sprite = generalPassive.Icon;
//            }
//        }
//
//        Image activeSkillIcon = randomSkills.transform.GetChild(2).gameObject.GetComponent<Image>();
//        var uiActiveSkillControl = activeSkillIcon.GetComponent<UIRandomSkillControl>();
//        SkillConfig activeSkill = GetActiveSkill(config.activeSkills, configInfo.ActiveSkill);
//        if (activeSkill)
//        {
//            uiActiveSkillControl.ActiveSkill = activeSkill;
//            if (activeSkill.Icon)
//            {
//                activeSkillIcon.sprite = activeSkill.Icon;
//            }
//        }
//    }
//
//    private EffectObject GetPassive(EffectObject[] passives, string passiveName)
//    {
//        if (String.IsNullOrEmpty(passiveName))
//        {
//            return null;
//        }
//        foreach (var passive in passives)
//        {
//            if (passive.name.Equals(passiveName))
//            {
//                return passive;
//            }
//        }
//        return null;
//    }
//
//    private SkillConfig GetActiveSkill(SkillConfig[] activeSkills, string activeSkill)
//    {
//        foreach (var skill in activeSkills)
//        {
//            if (activeSkill == skill.name)
//            {
//                return skill;
//            }
//        }
//        return null;
//    }
}
