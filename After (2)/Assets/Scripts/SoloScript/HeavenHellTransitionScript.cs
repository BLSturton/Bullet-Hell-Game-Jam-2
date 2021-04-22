using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeavenHellTransitionScript : MonoBehaviour
{
    public static HeavenHellTransitionScript instance;

    public static bool isInHellMode = false;

    public GameObject mask;

    public float timer = 10f;

    public Animator anim;

    public Text counter;
    public Text clearCounter;
    private float clearTimer;

    private bool firstUpdate = true;
    public AudioSource music;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(firstUpdate)
        {
            music.Play();
            firstUpdate = false;
        }
        timer -= Time.deltaTime;
        clearTimer -= Time.deltaTime;
        if(timer + Time.deltaTime >= 1.15 && timer < 1.15)
        {
            if (isInHellMode)
                anim.Play("ToHeaven");
            else
                anim.Play("ToHell");
        }

        clearCounter.text =  ((clearTimer>= 0)? "" + (int)(clearTimer + 0.99f):"");
        counter.text = "" + (int)(timer + 0.99f);

        if(timer <= 0)
        {
            timer += 10f;
            isInHellMode = !isInHellMode;
        }
    }

    public void ClearTimer()
    {
        clearTimer = 5f;
    }

}
