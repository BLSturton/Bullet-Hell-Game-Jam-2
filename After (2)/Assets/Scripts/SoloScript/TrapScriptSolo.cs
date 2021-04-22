using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapScriptSolo : MonoBehaviour
{
    public ProjectileShootingDataObject hellShotSource;
    public ProjectileShootingDataObject heavenShotSource;

    private ProjectileShootingDataObject hellShot;
    private ProjectileShootingDataObject heavenShot;

    Vector2 heavenDir = Vector2.right;

    public int heavenRotationDir = 1;

    private void Awake()
    {
        hellShot = Instantiate(hellShotSource);
        heavenShot = Instantiate(heavenShotSource);
        heavenDir = transform.rotation * heavenDir;
    }

    private void Update()
    {
        if (LevelGeneratorScript.cleared)
            return;
        hellShot.cooldownTimer -= Time.deltaTime;
        heavenShot.cooldownTimer -= Time.deltaTime;

        if(HeavenHellTransitionScript.isInHellMode)
        {
            if(hellShot.cooldownTimer <= 0)
            {
                hellShot.cooldownTimer = hellShot.cooldown;
                StartCoroutine(HellShotSalvo());
            }
        }
        else
        {
            if (heavenShot.cooldownTimer <= 0)
            {
                heavenShot.cooldownTimer = heavenShot.cooldown;
                SoloProjectileManager.ShootProjectile(transform.position, heavenShot, heavenDir);
                heavenDir = Quaternion.Euler(0, 0, heavenRotationDir * 25f) * heavenDir;
            }
        }
    }

    IEnumerator HellShotSalvo()
    {
        for(int x = 0;x < 4;x++)
        {
            SoloProjectileManager.ShootProjectile(transform.position, hellShot, new Vector2(1,1));
            yield return new WaitForSeconds(0.5f);
        }
    }
}
