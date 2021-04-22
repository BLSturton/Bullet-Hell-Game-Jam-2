using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoloPowerupManager : MonoBehaviour
{
    /*Singleton ref*/
    public static SoloPowerupManager instance;
    
    /*Powerup states*/
    public int chaosLevel = 0;
    public int pitchforkLevel = 0;
    public int lightBreezeLevel = 0;
    public int fieryRevengeLevel = 0;
    public int holyArrowLevel = 0;

    /*Display texts*/
    public Text holyArrowDisplay;
    public Text pitchforkDisplay;
    private void Awake()
    {
        instance = this;
    }

    //Can't think of any better way than just hardcoding for now
    public void AddPowerup(string name)
    {
        switch (name)
        {
            case ("HolyArrowPowerup"):
                holyArrowLevel++;
                break;
            case ("PitchforkPowerup"):
                pitchforkLevel++;
                break;
        }
        UpdateProjectileValues();
    }

    //Update projectile data values in solo shooting script to match powerups
    private void UpdateProjectileValues()
    {
        ProjectileShootingDataObject projectileDataAffected;
        projectileDataAffected = SoloCharacterShooting.instance.GetProjectileData("HolyArrow");
        projectileDataAffected.shotChance = Mathf.Min(0.1f * holyArrowLevel,1f);
        holyArrowDisplay.text = 100*projectileDataAffected.shotChance + "%";

        projectileDataAffected = SoloCharacterShooting.instance.GetProjectileData("Pitchfork");
        projectileDataAffected.cooldown = Mathf.Max(1f,projectileDataAffected.baseCooldown - 1 * pitchforkLevel);
        projectileDataAffected.cooldownTimer = 0f;
        projectileDataAffected.shotChance = (pitchforkLevel > 0) ? 1 : 0;
        pitchforkDisplay.text = (pitchforkLevel <= 0) ? "X" : projectileDataAffected.cooldown + " S";
    }

    public void ClearPowerups()
    {
        chaosLevel = 0;
        pitchforkLevel = 0;
        lightBreezeLevel = 0;
        fieryRevengeLevel = 0;
        holyArrowLevel = 0;
        UpdateProjectileValues();
    }
}
