using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoloPlayerEntity : SoloEntity
{ 
    //Singleton ref
    public static SoloPlayerEntity instance;
    /*Events*/
    public event Action OnPlayerDamagedEvent;

    private float invulnTime = 1.25f;
    public bool isInvuln;

    public Text healthBar;

    public override void Awake()
    {
        base.Awake();
        instance = this;
        UpdateHealthBar();
    }

    public override void TakeDamage(int damage, Vector2 kbDir, float kbVel)
    {
        if(!isInvuln)
        {
            if (damage > 0)
            {
                TempAudioManager.instance.PlayClip(0+ UnityEngine.Random.Range(0,2));
                TempHitOverlay.instance.PlayHurtAnim();
                isInvuln = true;
                Physics2D.IgnoreLayerCollision(10, 9);
                StartCoroutine(InvulnTime());
                OnPlayerDamagedEvent?.Invoke();
                //camera jerk
                CineMachineImpulseManager.instance.Impulse(kbDir.normalized * 2.5f);
            }
            base.TakeDamage(damage, kbDir, kbVel);
            UpdateHealthBar();
        }
 
    }

    private void UpdateHealthBar()
    {
        string healthDisplay = "";
        switch (health)
        {
            case 0:
                healthDisplay = "0";
                break;
            case 1:
                healthDisplay = "I";
                break;
            case 2:
                healthDisplay = "II";
                break;
            case 3:
                healthDisplay = "III";
                break;
            case 4:
                healthDisplay = "IV";
                break;
            case 5:
                healthDisplay = "V";
                break;
        }
        healthBar.text = healthDisplay;
    }
    
    IEnumerator InvulnTime()
    {
        yield return new WaitForSeconds(invulnTime);
        isInvuln = false;
        Physics2D.IgnoreLayerCollision(10, 9,false);
    }

    protected override void EntityDeath()
    {
        TempAudioManager.instance.PlayClip(2);
        LevelGeneratorScript.instance.Restart();
        SoloPowerupManager.instance.ClearPowerups();
        health = maxHealth;
        UpdateHealthBar();
    }
}
