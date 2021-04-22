using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloPowerupScript : MonoBehaviour
{
    public string powerupName;
    public ParticleSystem pickupParticles;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<SoloPowerupManager>() != null)
        {
            SoloPowerupManager.instance.AddPowerup(powerupName);
            pickupParticles.Play();
            GetComponent<AudioSource>().Play();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject,1f);
        }
    }
}
