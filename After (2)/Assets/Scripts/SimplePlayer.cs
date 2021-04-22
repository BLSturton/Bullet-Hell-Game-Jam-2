using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class SimplePlayer : NetworkBehaviour
{
    [SerializeField] private MainMenuManager _mainMenuManager = null;
    [SerializeField] private CharacterController controller = null;
    [SerializeField] private Camera myCamera = null;

    [SerializeField] private float MovementSpeed = 5f;

    public LayerMask WhatIsGround;
    public LayerMask WhatIsPlayer;

    [SerializeField] private bool IsGrounded;

    [SerializeField] private float TurningTime = 0.1f;
    private float TurnSmoothSpeed;

    [SerializeField] private float JumpHeight = 0.1f;
    [SerializeField] private float Gravity = -0.7f;

    public float Angle;

    private Vector2 movement;
    public Vector3 Velocity;

    public void Start()// this should run on both the server and the client so that there won't be any desync
    {
        myCamera = Camera.main;
        _mainMenuManager = Camera.main.GetComponent<MainMenuManager>();
        _mainMenuManager.inGame = true;

        _mainMenuManager.ActionsFromActionInputs[0].action.performed += context => InputMove(context);
        _mainMenuManager.ActionsFromActionInputs[1].action.performed += context => InputJump(context);
        _mainMenuManager.ActionsFromActionInputs[2].action.performed += context => InputPause(context);
    }

    [ClientCallback]
    void Update()
    {
        if (hasAuthority)
        {
            if (isLocalPlayer)
            {
                IsGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - 0.66f, transform.position.z), 0.4f, WhatIsGround);// place this in the server, with collitions?

                if (IsGrounded && Velocity.y < 0)
                {
                    Velocity.y = -0.05f;
                }

                MoveNow(movement);

                Velocity.y += Gravity * Time.fixedDeltaTime;
                controller.Move(Velocity);
            }
        }
    }

    [ClientCallback]
    public void InputPause(InputAction.CallbackContext _context)
    {
        if (hasAuthority)
        {
            if (isLocalPlayer)
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
                    {//meybe adding a "pause" in the menu that can also be rebinded
                        _mainMenuManager._playerInput.SwitchCurrentActionMap("Humanoid");
                        _mainMenuManager.OpenCloseCanvas("0 1 2 3 4 5 6");
                        _mainMenuManager.Back();
                    }
                }
            }
        }
    }

    [ClientCallback]
    public void InputJump(InputAction.CallbackContext _context)
    {
        if (hasAuthority)
        {
            if (isLocalPlayer)
            {
                if (_context.performed)
                {
                    if (IsGrounded)
                    {
                        Velocity.y = Mathf.Sqrt(JumpHeight * -2 * Gravity);
                    }

                    //MousesPos = Input.mousePosition;
                    //MousesPos.z = -Camera.main.transform.position.z;
                    //Vector3 diff = Camera.main.ScreenToWorldPoint(MousesPos) - transform.position;
                    //diff.Normalize();
                    //float rot = (Mathf.Atan2(diff.y, diff.x) + 3.9269f) * Mathf.Rad2Deg;
                    //Angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, rot, ref TurnSmoothSpeed, TurningTime);
                    //transform.rotation = Quaternion.Euler(0f, 0f, Angle);

                    //if (CanAttack)
                    //{
                    //    CanAttack = false;
                    //    SpawnObjectInServer();
                    //    Invoke("CanAttackAgain", 0.5f);
                    //}
                }
            }
        }
    }

    [ClientCallback]
    public void InputMove(InputAction.CallbackContext _context)
    {
        if (hasAuthority)
        {
            if (isLocalPlayer) 
            {
                movement = _context.ReadValue<Vector2>();
            }
        }
    }

    [ClientCallback]
    private void MoveNow(Vector2 direction)
    {
        Vector2 DirectionMove = direction.normalized;
        Vector3 motion = new Vector3(DirectionMove.x, 0, DirectionMove.y);
        controller.Move(motion * MovementSpeed * Time.deltaTime);

        if (direction.magnitude >= 0.1f)
        {
            float AngleToFace = (Mathf.Atan2(-direction.y, direction.x) * Mathf.Rad2Deg) + myCamera.transform.eulerAngles.y;
            float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, AngleToFace, ref TurnSmoothSpeed, TurningTime);
            transform.rotation = Quaternion.Euler(0f, Angle, 0f);
        }

        
    }

    public override void OnStopServer()
    {
        _mainMenuManager.OpenCloseCanvas("0 1 2 3 4 5 6 7 8");
    }

    public override void OnStopClient()
    {
        _mainMenuManager.OpenCloseCanvas("0 1 2 3 4 5 6 7 8");
    }

    //[Command]
    //public void SpawnObjectInServer()
    //{
    //    MousesPos = Input.mousePosition;
    //    MousesPos.z = -Camera.main.transform.position.z;
    //    Vector3 diff = Camera.main.ScreenToWorldPoint(MousesPos) - transform.position;
    //    diff.Normalize();
    //    float rot = (Mathf.Atan2(diff.y, diff.x) + 3.9269f) * Mathf.Rad2Deg;
    //    Angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, rot, ref TurnSmoothSpeed, TurningTime);//try something more efficient than this
    //    GameObject Spike = Instantiate(Bullet, transform.position, Quaternion.Euler(0f, 0f, Angle));
    //    NetworkServer.Spawn(Spike);
    //}

    //[Server]
    //private void OnControllerColliderHit(ControllerColliderHit collision)
    //{
    //    collition can only be checked in server
    //}

    //[Server]
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag.Equals("MovingPlatform"))
    //    {
    //        this.transform.parent = collision.transform;
    //        //SavePos = transform.localPosition;
    //    }
    //    if (collision.gameObject.tag.Equals("Win"))
    //    {
    //        YouWin = true;
    //        TimeStart = Time.time;
    //        Invoke("GoNextLevel", 2.5f);
    //        //SavePos = transform.localPosition;
    //    }
    //}
}
