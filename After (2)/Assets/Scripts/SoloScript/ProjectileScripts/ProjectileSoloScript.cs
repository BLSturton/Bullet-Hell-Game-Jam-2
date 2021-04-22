using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSoloScript : MonoBehaviour
{
    /*Components*/
    public Rigidbody2D rb;
    public Collider2D coll;
    public SpriteRenderer ren;
    /*Projectile fields*/
    public float velocity;
    public int damage;
    public float knockbackVel;

    public LayerMask targetMask;
    /*Tracking fields*/
    public bool tracking;
    public float angularVelocity;
    public LayerMask trackingMask;

    /*Visual effects*/
    public ParticleSystem hitParticles;

    private void OnEnable()
    {
        rb.velocity = transform.rotation * Vector2.right * velocity;
    }

    /*Projectile collision logic*/
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(targetMask.Contains(collision.gameObject.layer))
        {
            TargetHit(collision);
        }
    }

    protected virtual void TargetHit(Collision2D collision)
    {
        SetProjectilePhysics(false);
        tracking = false;
        SoloEntity entity = collision.gameObject.GetComponent<SoloEntity>();
        transform.rotation = Quaternion.Euler(0, 0, (-collision.relativeVelocity).AngleDegrees());
        if(entity != null)
        {
            entity.TakeDamage(damage, -collision.relativeVelocity, knockbackVel);
        }
        //Particle effects
        if (hitParticles != null)
            hitParticles.Play();

        //No projectile pooling yet, so destroy for now
        Destroy(gameObject,0.8f);
    }

    protected void SetProjectilePhysics(bool val)
    {
        coll.enabled = val;
        rb.simulated = val;
        ren.enabled = val;
    }

    private void Update()
    {
        //If projectile tracks entities
        if(tracking)
        {
            //Circlecasts for enemies
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 10, trackingMask);

            /*whatever the fuck this is, please clean it up sometime*/
            if(hits.Length > 0)
            {
                float minDist = 10000f;
                Vector3 targetPosition = hits[0].transform.position;
                foreach (Collider2D hit in hits)
                {
                    float sqDist = (transform.position - hit.transform.position).sqrMagnitude;
                    if(sqDist > minDist)
                    {
                        minDist = sqDist;
                        targetPosition = hit.transform.position;
                    }
                }

                Vector2 vectorToEnemy = targetPosition - transform.position;
                float targetAngle = vectorToEnemy.AngleDegrees();
                float cAngle = transform.rotation.eulerAngles.z;
                float deltaAngle = Mathf.DeltaAngle(cAngle, targetAngle);
                float rotationDir = Mathf.Sign(deltaAngle);

                transform.rotation = Quaternion.Euler(0, 0, cAngle + rotationDir * angularVelocity * Time.deltaTime);
                rb.velocity = transform.rotation * Vector2.right * velocity;
            }
        }
    }

}
