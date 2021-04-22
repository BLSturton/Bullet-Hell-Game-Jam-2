using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloPlayerAnimationScript : MonoBehaviour
{
    /*Component refs*/
    private Animator anim;
    public Rigidbody2D referenceRb;
    //Anim state field
    private string currentState = "IdleAnim";

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if(SoloPlayerEntity.instance.isInvuln)
        {
            PlayState("InvulnAnim");
        }
        else if(referenceRb.velocity.sqrMagnitude > 0.1f)
        {
            PlayState("WalkAnim");
        }
        else
        {
            PlayState("IdleAnim");
        }
    }

    void PlayState(string state)
    {
        if (state != currentState)
        {
            anim.Play(state);
            currentState = state;
        }
    }
}
