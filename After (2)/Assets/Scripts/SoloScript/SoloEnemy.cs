using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloEnemy : MonoBehaviour
{
    [SerializeField] private Rigidbody2D MyBody;

    [SerializeField] private float MovementDamping = 0.3f; // limits the speed, the lower it is the faster you are.
    [SerializeField] private float MovementDampingWhenStopping = 0.6f;
    [SerializeField] private float MovementDampingWhenTurning = 0.8f;

    [SerializeField] private float EnemyMaxSpeed = 40f;// make this feel good, i think too high
    [SerializeField] private float MoveInX;
    [SerializeField] private float MoveInY;

    private Vector2 dirToPlayerNorm;

    private float TurnSmoothSpeed;
    public float RotateTo = 0;

    [SerializeField] private Transform EnemyObject = null;
    [SerializeField] private SpriteRenderer NormalSprite = null;
    [SerializeField] private SpriteRenderer TransformedSprite = null;

    [SerializeField] private Transform Player = null;
    [SerializeField] private Transform Gun = null;

    /*Shooting fields*/
    //Dont edit the source SO timers
    public ProjectileShootingDataObject heavenShotSource;
    public ProjectileShootingDataObject hellShotSource;

    private ProjectileShootingDataObject heavenShot;
    private ProjectileShootingDataObject hellShot;


    public void Start()
    {
        MyBody = gameObject.GetComponent<Rigidbody2D>();
        MyBody.simulated = true;

        Player = SoloPlayerEntity.instance.gameObject.transform;

        hellShot = Instantiate(hellShotSource);
        heavenShot = Instantiate(heavenShotSource);
    }

    public void Change()
    {
        NormalSprite.sprite = TransformedSprite.sprite;
    }

    private void Update()
    {
        //Timers
        hellShot.cooldownTimer -= Time.deltaTime;
        heavenShot.cooldownTimer -= Time.deltaTime;

        TryShoot();
    }

    void FixedUpdate()
    {
        Vector2 direction = (Player.position - transform.position).normalized;
        dirToPlayerNorm = direction;

        if (direction.x > 0)
        {
            MoveInX = 1;
            RotateTo = 180;
        }
        else if (direction.x < 0)
        {
            MoveInX = -1;
            RotateTo = 0;
        }
        else
        {
            MoveInX = 0;
        }

        if (direction.y > 0)
        {
            MoveInY = 1;
        }
        else if (direction.y < 0)
        {
            MoveInY = -1;
        }
        else
        {
            MoveInY = 0;
        }

        float rot = (Mathf.Atan2(direction.y, direction.x) + 3.9269f) * Mathf.Rad2Deg;
        Gun.rotation = Quaternion.Euler(0f, 0f, rot - 45);

        float Angl = Mathf.SmoothDampAngle(EnemyObject.eulerAngles.y, RotateTo, ref TurnSmoothSpeed, 0.33f);
        EnemyObject.rotation = Quaternion.Euler(0f, Angl, 0f);

        if (MyBody.velocity.magnitude > EnemyMaxSpeed)
        {
            MyBody.velocity = Vector3.ClampMagnitude(MyBody.velocity, EnemyMaxSpeed);
        }

        //_animator.SetBool("Running", MoveInX != 0);

        float HorizontalVelocity = MyBody.velocity.x;
        HorizontalVelocity += (MoveInX) / 9.00f;

        if (Mathf.Abs((MoveInX) / 9.00f) < 0.01f)
            HorizontalVelocity *= Mathf.Pow(1f - MovementDampingWhenStopping, Time.fixedDeltaTime * 10.0000f);
        else if (Mathf.Sign((MoveInX) / 9.00f) != Mathf.Sign(HorizontalVelocity))
            HorizontalVelocity *= Mathf.Pow(1f - MovementDampingWhenTurning, Time.fixedDeltaTime * 10.0000f);
        else
            HorizontalVelocity *= Mathf.Pow(1f - MovementDamping, Time.fixedDeltaTime * 10.0000f);

        float VerticalVelocity = MyBody.velocity.y;
        VerticalVelocity += (MoveInY) / 9.00f;

        if (Mathf.Abs((MoveInY) / 9.00f) < 0.01f)
            VerticalVelocity *= Mathf.Pow(1f - MovementDampingWhenStopping, Time.fixedDeltaTime * 10.0000f);
        else if (Mathf.Sign((MoveInY) / 9.00f) != Mathf.Sign(VerticalVelocity))
            VerticalVelocity *= Mathf.Pow(1f - MovementDampingWhenTurning, Time.fixedDeltaTime * 10.0000f);
        else
            VerticalVelocity *= Mathf.Pow(1f - MovementDamping, Time.fixedDeltaTime * 10.0000f);

        MyBody.velocity = new Vector2(HorizontalVelocity * 0.9f, VerticalVelocity * 0.9f);

        
    }

    void TryShoot()
    {
        if(HeavenHellTransitionScript.isInHellMode)
        {
            if(hellShot.cooldownTimer <= 0)
            {
                hellShot.cooldownTimer = hellShot.cooldown + Random.Range(-0.15f,0.15f);
                SoloProjectileManager.ShootProjectile(transform.position, hellShot, dirToPlayerNorm);
            }
        }
        else
        {
            if (heavenShot.cooldownTimer <= 0)
            {
                heavenShot.cooldownTimer = heavenShot.cooldown + Random.Range(-0.25f, 0.25f);
                StartCoroutine(ShootHeavenStringShot());
            }
        }
    }

    IEnumerator ShootHeavenStringShot()
    {
        for(int x = 0;x < 3;x++)
        {
            SoloProjectileManager.ShootProjectile(transform.position, heavenShot, dirToPlayerNorm);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
