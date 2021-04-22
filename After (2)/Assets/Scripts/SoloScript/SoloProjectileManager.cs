using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoloProjectileManager
{
    public static bool ShootProjectile(Vector3 position, ProjectileShootingDataObject projectileData, Vector2 directionNorm)
    {
        return ShootProjectile(position, projectileData, directionNorm, false);
    }

    public static bool ShootProjectile(Vector3 position, ProjectileShootingDataObject projectileData, Vector2 directionNorm,bool cameraRecoil)
    {
        float angle = directionNorm.AngleDegrees();
        //Rolls for chance tos hoot
        float value = Random.Range(0, 1f);
        if (value <= projectileData.shotChance)
        {
            //Calculates Arc angle for shotgun style bursts
            float startAngle = angle - projectileData.spreadDegrees / 2f;
            float interval;
            if (projectileData.count != 1)
                interval = projectileData.spreadDegrees / (projectileData.count - 1);
            else
                interval = 0f;

            //Projectile creation along arc angle
            for (int x = 0; x < projectileData.count; x++)
            {
                float currentAngle = startAngle + x * interval;
                //Should use pooling sometime, instantiating do be costly :(
                GameObject.Instantiate(projectileData.projectilePrefab, position, Quaternion.Euler(0, 0, currentAngle));
            }
            //Camera go brrrrr
            if (cameraRecoil)
                CineMachineImpulseManager.instance.Impulse(-directionNorm * projectileData.cameraRecoil);

        }
        else
            return false;
        return true;
    }
}
