using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoloCharacterShooting : MonoBehaviour
{
    /*Singleton*/
    public static SoloCharacterShooting instance;

    /*Input asset refs*/
    public InputActionAsset inputAsset;

    /*Component refs*/
    private SoloCharacter charScript;
    private Camera mainCamera;

    /*Weapon visual objects*/
    public Transform weaponTransform;
    public Transform barrelTip;
    public SpriteRenderer weaponSprite;
    /*Shooting object&prefab refs*/

    public ProjectileShootingDataObject[] projectiles;
    private int projTypeCount;

    /*States*/
    [HideInInspector] public bool isShooting = false;

    private void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        projTypeCount = projectiles.Length;
        //Replaces references of the source projectile scriptableobjects with local copies
        for (int x = 0; x < projTypeCount; x++)
        {
            projectiles[x] = Instantiate(projectiles[x]);
        }

        instance = this;
        charScript = GetComponent<SoloCharacter>();
        /*Input actions*/
        inputAsset.FindActionMap("Humanoid").actions[1].performed += context => { isShooting = !isShooting;ResetWeaponRotation(); };
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        /*Timer updates*/
        for(int x = 0;x < projTypeCount;x++)
        { 
            projectiles[x].cooldownTimer -= Time.deltaTime;
        }
        /*Shooting logic*/
        if (isShooting)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 vectorToMouseNorm = (mousePosition - (Vector2)transform.position).normalized;
            Vector2 barrelToMouseNorm = (mousePosition - (Vector2)barrelTip.transform.position).normalized;

            RotateWeaponToMouse(vectorToMouseNorm);
            SetPlayerFacing(vectorToMouseNorm);

            ShootAction(barrelToMouseNorm);
        }
    }

    //Assumes left/right flipping is handeled by soloCharacter script through transform scale
    void RotateWeaponToMouse(Vector2 directionNorm)
    {
        Vector3 currentGlobalRotation = weaponTransform.rotation.eulerAngles;
        float offset = (weaponTransform.lossyScale.x <= 0) ? 180f : 0f;
        weaponTransform.rotation = Quaternion.Euler(currentGlobalRotation.x, currentGlobalRotation.y, directionNorm.AngleDegrees()+offset);
    }

    void ResetWeaponRotation()
    {
        weaponTransform.rotation = Quaternion.identity;
        prevFacing = 0;
    }

    int prevFacing;
    void SetPlayerFacing(Vector2 directionNorm)
    {
        int dir = (int)Mathf.Sign(directionNorm.x);
        if (dir != prevFacing && dir != 0)
        {
            charScript.RotateToProperty = dir;
            charScript.RotationTimeProperty = Time.time;
            prevFacing = dir;
        }
    }

    void ShootAction(Vector2 directionNorm)
    {
        //Iterates over each projectile type and shoots if their cooldown is up
        for(int x = 0;x < projTypeCount;x++)
        {
            ProjectileShootingDataObject projectileData = projectiles[x];
            if(projectileData.cooldownTimer <= 0)
            {
                projectileData.cooldownTimer = projectileData.cooldown;
                bool shot = SoloProjectileManager.ShootProjectile(barrelTip.position,projectileData,directionNorm,true);
                
                //Hard coding sound clips for now, add data to projectile later
                if(shot)
                {
                    switch (x)
                    {
                        case 0:
                            TempAudioManager.instance.PlayClip(3);
                            break;
                        case 1:
                            TempAudioManager.instance.PlayClip(4);
                            break;
                        case 2:
                            TempAudioManager.instance.PlayClip(5);
                            break;
                    }
                }
            }
        }
    }

    public ProjectileShootingDataObject GetProjectileData(string name)
    {
        for(int x = 0;x < projTypeCount;x++)
        {
            if(projectiles[x].projectileName.Equals(name))
            {
                return projectiles[x];
            }
        }
        return null;
    }
}
