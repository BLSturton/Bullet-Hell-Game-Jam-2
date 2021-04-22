using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelGeneratorScript : MonoBehaviour
{
    /*Singleton r ef*/
    public static LevelGeneratorScript instance;

    /*Level managment*/
    public GameObject[] levels;
    private int size;


    private GameObject currentLevelObject;
    /*Powerup management*/
    public GameObject[] powerups;
    private int powerupCount;
    /*States*/
    private float currentEnemyCount;
    public int currentLevel = 0;
    public static bool cleared;
    /*Text*/
    public Text levelText;
    private void Awake()
    {
        size = levels.Length;
        powerupCount = powerups.Length;
        instance = this;
    }
    private void Start()
    {
        LoadNewLevel();
    }
    public void LoadNewLevel()
    {
        cleared = false;
        int roll = Random.Range(0, size);
        currentLevel++;
        levelText.text = "F:" + currentLevel;
        currentLevelObject = Instantiate(levels[roll]);
        StartCoroutine(MovePlayerToCenter());
    }
    IEnumerator MovePlayerToCenter()
    {
        for(int x = 0;x <= 20;x++)
        {
            SoloCharacterShooting.instance.gameObject.transform.position = Vector2.Lerp(SoloCharacterShooting.instance.gameObject.transform.position, Vector2.zero, 0.25f);
            yield return new WaitForFixedUpdate();
        }
    }
    public void EnemyKilled()
    {
        currentEnemyCount--;
        if(currentEnemyCount <= 0 && !cleared)
        {
            cleared = true;
            StartCoroutine(LevelCleared());
        }
    }

    public void EnemySpawned()
    {
        currentEnemyCount++;
    }

    IEnumerator LevelCleared()
    {
        levelText.text = "CLEAR";
        if (currentLevel % 2 == 0)
        {
            SpawnPowerups();
        }
        HeavenHellTransitionScript.instance.ClearTimer();
        yield return new WaitForSeconds(5f);
        ClearCurrentLevel();
        LoadNewLevel();
    }


    void ClearCurrentLevel()
    {
        currentEnemyCount = 0;
        Destroy(currentLevelObject);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject g in enemies)
            Destroy(g);
    }
    void SpawnPowerups()
    {
        int roll = Random.Range(0, powerupCount);
        Instantiate(powerups[roll], Vector2.zero,Quaternion.identity);
    }

    public void Restart()
    {
        currentLevel = 0;
        ClearCurrentLevel();
        LoadNewLevel();
    }
}
