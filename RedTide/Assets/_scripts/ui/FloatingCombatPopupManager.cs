using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCombatPopupManager : MonoBehaviour
{
    public static FloatingCombatPopupManager Instance;
    
    public GameObject Canvas;
    public GameObject DamagePopup;
    public GameObject DamageTakenPopup;
    public GameObject CriticalDamagePopup;
    public GameObject TakenCriticalDamagePopup;
    public GameObject HealingPopup;
    public GameObject CriticalHealingPopup;
    public GameObject DamageAbsorbPopup;
    public GameObject InvinciblePopup;
    public GameObject MissPopup;
    public GameObject GoldPopup;
    public GameObject TextPopup;
    public Material Grayscale;

    // Use this for initialization
	void Start () {
	    DoSingleton();
	}

    private void DoSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this.gameObject)
        {
            Destroy(this.gameObject);
        }
    }
	    
    public void ProcessFloatingCombatPopup(FloatingCombatPopupType popupType, float value, GameUnit target)
    {
        GameObject floatingPopup = null;
        switch (popupType)
        {
            case FloatingCombatPopupType.Damage:
                floatingPopup = Instantiate(DamagePopup);
                break;
            case FloatingCombatPopupType.DamageTaken:
                floatingPopup = Instantiate(DamageTakenPopup);
                break;
            case FloatingCombatPopupType.CriticalDamage:
                floatingPopup = Instantiate(CriticalDamagePopup);
                break;
            case FloatingCombatPopupType.CriticalDamageTaken:
                floatingPopup = Instantiate(TakenCriticalDamagePopup);
                break;
            case FloatingCombatPopupType.Healing:
                floatingPopup = Instantiate(HealingPopup);
                break;
            case FloatingCombatPopupType.CriticalHealing:
                floatingPopup = Instantiate(CriticalHealingPopup);
                break;
            case FloatingCombatPopupType.DamageAbsorb:
                break;
            case FloatingCombatPopupType.Invincible:
                floatingPopup = Instantiate(InvinciblePopup);
                break;
            case FloatingCombatPopupType.Miss:
                floatingPopup = Instantiate(MissPopup);
                break;
            case FloatingCombatPopupType.Gold:
                break;
        }

        if (floatingPopup)
        {
            floatingPopup.transform.SetParent(Canvas.transform, false);
            DamagePopupControl damagePopupControl = floatingPopup.GetComponent<DamagePopupControl>();
            damagePopupControl.target = target;
            if (popupType != FloatingCombatPopupType.Invincible && popupType != FloatingCombatPopupType.Miss && popupType != FloatingCombatPopupType.DamageAbsorb)
            {
                damagePopupControl.SetDamage(value);
            }
        }
    }

    public void ProcessFloatingText(string text, GameUnit target)
    {
        GameObject floatingPopup = Instantiate(TextPopup);
        if (floatingPopup)
        {
            floatingPopup.transform.SetParent(Canvas.transform, false);
            DamagePopupControl damagePopupControl = floatingPopup.GetComponent<DamagePopupControl>();
            damagePopupControl.target = target;
            damagePopupControl.SetText(text);
        }
    }

}

public enum FloatingCombatPopupType
{
    Damage,
    DamageTaken,
    CriticalDamage,
    CriticalDamageTaken,
    Healing,
    CriticalHealing,
    DamageAbsorb,
    Invincible,
    Miss,
    Gold
}
