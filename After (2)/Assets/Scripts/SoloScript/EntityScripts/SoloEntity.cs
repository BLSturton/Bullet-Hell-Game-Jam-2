using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoloEntity : MonoBehaviour
{
    /*Component refs*/
    protected Rigidbody2D entityRb;
    /*Entity stat fields*/
    [SerializeField]protected int health = 0;
    public int maxHealth = 0;

    public virtual void Awake()
    {
        entityRb = gameObject.GetComponent<Rigidbody2D>();
    }


    public virtual void TakeDamage(int damage, Vector2 kbDir, float kbVel)
    {
        entityRb.velocity = kbDir.normalized * kbVel;
        if (damage == 0)
            return;
        health -= damage;
        if (health <= 0)
            EntityDeath();
    }

    protected abstract void EntityDeath(); /*Death logic*/

}
