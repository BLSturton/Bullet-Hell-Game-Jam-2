using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempHitOverlay : MonoBehaviour
{
    public static TempHitOverlay instance;
    private Animator anim;
    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
    }

    public void PlayHurtAnim()
    {
        anim.Play("Hurt",0,0f);
    }
}
