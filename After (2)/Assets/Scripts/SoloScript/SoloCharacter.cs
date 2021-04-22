using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoloCharacter : MonoBehaviour
{
    //[SerializeField] private Animator _animator = null;
    public PlayerInput _playerInput = null;
    private InputActionMap map;

    [SerializeField] private Rigidbody2D PlayerBody;
    [SerializeField] private Camera myCamera = null;

    [SerializeField] private float MovementDamping = 0.3f; // limits the speed, the lower it is the faster you are.
    [SerializeField] private float MovementDampingWhenStopping = 0.6f;
    [SerializeField] private float MovementDampingWhenTurning = 0.8f;

    [SerializeField] private float MoveInX;
    [SerializeField] private float MoveInY;

    [SerializeField] private float PlayerMaxSpeed = 40f;// make this feel good, i think too high
    [SerializeField] private float SmoothSpeed = 0f;

    [SerializeField] private GameObject Bullet = null;
    [SerializeField] private Transform Player = null;
    [SerializeField] private Transform ShootingHand = null;
    [SerializeField] private Transform Hand = null;

    private float RotationTime;
    private int RotateTo = 1;

    [HideInInspector] public int RotateToProperty { set => RotateTo = value; get => RotateTo; }
    [HideInInspector] public float RotationTimeProperty { set => RotationTime = value; get => RotationTime; }

    private bool Cooldown;
    private bool Shooting;
    private bool ChangedP = true;
    private bool ChangedN = true;
    private bool ChangedL = true;
    private bool ChangedR = true;

    public LayerMask WhatIsGround;
    public LayerMask WhatIsPlayer;

    private Vector2 movement;
    public Vector3 Velocity;

    // Start is called before the first frame update
    void Start()
    {
        PlayerBody = gameObject.GetComponent<Rigidbody2D>();
        PlayerBody.simulated = true;//on the server only
        RotationTime = Time.time;

        myCamera = Camera.main;

        map = _playerInput.actions.FindActionMap("Humanoid");
        map.actions[0].performed += context => InputMove(context);
        map.actions[1].performed += context => InputAttack(context);
        //map.actions[2].performed += context => InputPause(context);
    }

    // Update is called once per frame
    void Update()
    {
        Player.localScale = new Vector3(Mathf.Lerp(Player.localScale.x, RotateTo, (Time.time - RotationTime) * 3), 1, 1);

        //_mainMenuManager.CameraTransform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        MoveNow(movement);
        if (PlayerBody.velocity.magnitude > PlayerMaxSpeed)
        {
            PlayerBody.velocity = Vector3.ClampMagnitude(PlayerBody.velocity, PlayerMaxSpeed);
        }
    }

    private void LateUpdate()
    {
        if (transform.position.y > 100)
        {
            transform.position = new Vector3(-1.29f, 6.2f, 0);
        }

        /*if (Shooting)
        {
            Vector3 MousesPos = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, -myCamera.transform.position.z);
            Vector3 diff = (myCamera.ScreenToWorldPoint(MousesPos) - new Vector3(transform.position.x, transform.position.y, transform.position.z)).normalized;
            float rot = (Mathf.Atan2(diff.y, diff.x) + 3.9269f) * Mathf.Rad2Deg;
            ShootingHand.rotation = Quaternion.Euler(0f, 0f, rot - 135);
            Hand.rotation = Quaternion.Euler(0f, 0f, rot - 45);

            if (diff.x > 0.00f)
            {
                RotateTo = 1;
                ChangedP = true;
                if (ChangedN)
                {
                    ChangedN = false;
                    RotationTime = Time.time;
                }
                if (movement.x <= 0.00f)
                {
                    //_animator.SetFloat("AnimationSpeed", -1);
                }
            }
            else
            {
                RotateTo = -1;
                ChangedN = true;
                if (ChangedP)
                {
                    ChangedP = false;
                    RotationTime = Time.time;
                }
                if (movement.x > 0.00f)
                {
                    //_animator.SetFloat("AnimationSpeed", -1);
                }
            }

            if (!Cooldown)
            {
                //GameObject Spike = Instantiate(Bullet, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.Euler(0f, 0f, rot));
                Cooldown = true;
                StartCoroutine(DoneCooldown());
            }
        }*/
    }
/*
    public void InputPause(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            if (_mainMenuManager._playerInput.currentActionMap.name.ToString() == "Humanoid")
            {
                _mainMenuManager._playerInput.SwitchCurrentActionMap("Menu");
                _mainMenuManager.OpenCloseCanvas("1 0");
                _mainMenuManager.OpenCloseCanvas("2 0");
            }
            else if (_mainMenuManager._playerInput.currentActionMap.name.ToString() == "Menu")//this cannot be called because when we pause the action map becames "menu" and this is not present in the "menu"
            {
                _mainMenuManager._playerInput.SwitchCurrentActionMap("Humanoid");
                _mainMenuManager.OpenCloseCanvas("0 1 2 3 4 5 6");
                _mainMenuManager.Back();
            }
        }
    }
*/
    public void InputAttack(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            Shooting = !Shooting;
            RotationTime = Time.time;
        }
    }

    public void InputMove(InputAction.CallbackContext _context)
    {
        movement = _context.ReadValue<Vector2>();
    }

    private void MoveNow(Vector2 direction)
    {
        //_animator.SetBool("Running", direction.magnitude >= 0.2f);

        float HorizontalVelocity = PlayerBody.velocity.x;
        HorizontalVelocity += (MoveInX) / 9.00f;

        if (Mathf.Abs((MoveInX) / 9.00f) < 0.01f)
            HorizontalVelocity *= Mathf.Pow(1f - MovementDampingWhenStopping, Time.fixedDeltaTime * 10.0000f);
        else if (Mathf.Sign((MoveInX) / 9.00f) != Mathf.Sign(HorizontalVelocity))
            HorizontalVelocity *= Mathf.Pow(1f - MovementDampingWhenTurning, Time.fixedDeltaTime * 10.0000f);
        else
            HorizontalVelocity *= Mathf.Pow(1f - MovementDamping, Time.fixedDeltaTime * 10.0000f);

        float VerticalVelocity = PlayerBody.velocity.y;
        VerticalVelocity += (MoveInY) / 9.00f;

        if (Mathf.Abs((MoveInY) / 9.00f) < 0.01f)
            VerticalVelocity *= Mathf.Pow(1f - MovementDampingWhenStopping, Time.fixedDeltaTime * 10.0000f);
        else if (Mathf.Sign((MoveInY) / 9.00f) != Mathf.Sign(VerticalVelocity))
            VerticalVelocity *= Mathf.Pow(1f - MovementDampingWhenTurning, Time.fixedDeltaTime * 10.0000f);
        else
            VerticalVelocity *= Mathf.Pow(1f - MovementDamping, Time.fixedDeltaTime * 10.0000f);

        PlayerBody.velocity = new Vector2(HorizontalVelocity * (1.00f - SmoothSpeed), VerticalVelocity * (1.00f - SmoothSpeed));

        if (direction.x > 0)
        {
            ChangedL = true;
            if (ChangedR)
            {
                ChangedR = false;
                RotationTime = Time.time;
            }
            if(!SoloCharacterShooting.instance.isShooting)
                RotateTo = 1;
            MoveInX = 1;
        }
        else if (direction.x < 0)
        {
            ChangedR = true;
            if (ChangedL)
            {
                ChangedL = false;
                RotationTime = Time.time;
            }
            if (!SoloCharacterShooting.instance.isShooting)
                RotateTo = -1;
            MoveInX = -1;
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
    }

    private IEnumerator DoneCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        Cooldown = false;
    }

    private IEnumerator DoneSmooth()
    {
        yield return new WaitForSeconds(0.4f);
        SmoothSpeed = 0f;
    }
}
