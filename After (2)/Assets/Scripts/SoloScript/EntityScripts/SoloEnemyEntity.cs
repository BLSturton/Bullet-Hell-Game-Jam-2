using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloEnemyEntity : SoloEntity
{
    private void Awake()
    {
        base.Awake();
        LevelGeneratorScript.instance.EnemySpawned();
        health += LevelGeneratorScript.instance.currentLevel;//Health scaling
    }

    public override void TakeDamage(int damage, Vector2 kbDir, float kbVel)
    {
        base.TakeDamage(damage, kbDir, kbVel);
        if(damage > 0)
        {
            if(HeavenHellTransitionScript.isInHellMode)
            {
                TempAudioManager.instance.PlayClip(8);
            }
            else
            {
                TempAudioManager.instance.PlayClip(6);
            }
        }
    }

    protected override void EntityDeath()
    {
        /*Enemy death logic*/
        LevelGeneratorScript.instance.EnemyKilled();
        if (HeavenHellTransitionScript.isInHellMode)
        {
            TempAudioManager.instance.PlayClip(9);
        }
        else
        {
            TempAudioManager.instance.PlayClip(7);
        }
        Destroy(gameObject);
    }
}
