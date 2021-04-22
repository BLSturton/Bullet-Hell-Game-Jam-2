using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "ScriptableObjects/ProjectileData", order = 1)]
[System.Serializable]
public class ProjectileShootingDataObject : ScriptableObject
{
    public string projectileName;
    public GameObject projectilePrefab;
    public float shotChance;
    public float baseCooldown;
    [HideInInspector]public float cooldown;
    public int count;
    public float spreadDegrees;
    public float cameraRecoil;
    [HideInInspector] public float cooldownTimer;
    private void OnEnable()
    {
        cooldown = baseCooldown;
    }
}
