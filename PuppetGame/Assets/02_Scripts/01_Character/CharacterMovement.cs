using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Character/Movement")]
[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(BoxCollider2D))]
public class CharacterMovement : MonoBehaviour
{
    //Scriptable object which holds all the player's movement parameters. If you don't want to use it
    //just paste in all the parameters, though you will need to manuly change all references in this script
    public PlayerRunData Data;

    #region COMPONENTS
    public Rigidbody2D RB { get; private set; }
    Animator m_animator;

    //Script to handle all player animations, all references can be safely removed if you're importing into your own project.
    #endregion

    #region STATE PARAMETERS
    //Variables control the various actions the player can perform at any time.
    //These are fields which can are public allowing for other sctipts to read them
    //but can only be privately written to.
    [SerializeField] bool _isFacingRight = true;
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    //public bool IsWallJumping { get; private set; }
    public bool IsSliding { get; private set; }

    //Timers
    public float LastOnGroundTime { get; private set; }
    //public float LastOnWallTime { get; private set; }
    //public float LastOnWallRightTime { get; private set; }
    //public float LastOnWallLeftTime { get; private set; }

    //Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;

    /*    //Wall Jump
        private float _wallJumpStartTime;
        private int _lastWallJumpDir;*/

    #endregion

    #region INPUT PARAMETERS
    private Vector2 _currentMoveInput;
    private InputAction _moveInput;
    private InputAction _jumpInput;
    public float LastPressedJumpTime { get; private set; }
    #endregion

    #region CHECK PARAMETERS
    //Set all of these up in the inspector
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    /*    [Space(5)]
        [SerializeField] private Transform _frontWallCheckPoint;
        [SerializeField] private Transform _backWallCheckPoint;
        [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);*/

    private int _jumpsLeft;

    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    #region Copies from SO
    // runtime copy (could be its own struct or separate variables)
/*    [HideInInspector] public float runMaxSpeed;
    [HideInInspector] public float runAccelAmount;
    [HideInInspector] public float runDeccelAmount;
    [HideInInspector] public float accelInAir;
    [HideInInspector] public float deccelInAir;
    [HideInInspector] public float jumpHangAccelerationMult;
    [HideInInspector] public float jumpHangMaxSpeedMult;
    [HideInInspector] public bool doConserveMomentum;
    [HideInInspector] public float jumpHangTimeThreshold;*/
    #endregion

    #region Consts
    // runtime copy (could be its own struct or separate variables)
    float c_runMaxSpeed;
    float c_runAccelAmount;
    float c_runDeccelAmount;
    float c_accelInAir;
    float c_deccelInAir;
    float c_jumpHangAccelerationMult;
    float c_jumpHangMaxSpeedMult;
    bool c_doConserveMomentum;
    float c_jumpHangTimeThreshold;
    #endregion

    private void OnEnable()
    {
        InputSystem.actions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        ResetSOValuesToInit();
        InputSystem.actions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();

        CopyFromSO();
    }

    private void Start()
    {
        //Assigning inputs
        _moveInput = InputSystem.actions.FindAction("Move");
        _moveInput.performed += OnMoveInput;
        _moveInput.canceled += OnMoveInput;

        _jumpInput = InputSystem.actions.FindAction("Jump");
        _jumpInput.performed += OnJumpUpInput;
        _jumpInput.canceled += OnJumpUpInput; // double jump?

        //Setting up defaults 
        SetGravityScale(Data.gravityScale);

        IsFacingRight = _isFacingRight;

    }

    public void CopyFromSO()
    {/*
        runMaxSpeed = Data.runMaxSpeed;
        runAccelAmount = Data.runAccelAmount;
        runDeccelAmount = Data.runDeccelAmount;
        accelInAir = Data.accelInAir;
        deccelInAir = Data.deccelInAir;
        jumpHangAccelerationMult = Data.jumpHangAccelerationMult;
        jumpHangMaxSpeedMult = Data.jumpHangMaxSpeedMult;
        doConserveMomentum = Data.doConserveMomentum;
        jumpHangTimeThreshold = Data.jumpHangTimeThreshold;*/

        c_runMaxSpeed = Data.runMaxSpeed;
        c_runAccelAmount = Data.runAccelAmount;
        c_runDeccelAmount = Data.runDeccelAmount;
        c_accelInAir = Data.accelInAir;
        c_deccelInAir = Data.deccelInAir;
        c_jumpHangAccelerationMult = Data.jumpHangAccelerationMult;
        c_jumpHangMaxSpeedMult = Data.jumpHangMaxSpeedMult;
        c_doConserveMomentum = Data.doConserveMomentum;
        c_jumpHangTimeThreshold = Data.jumpHangTimeThreshold;
    }

    public void ResetSOValuesToInit()
    {
        Data.runMaxSpeed = c_runMaxSpeed;
        Data.runAccelAmount = c_runAccelAmount;
        Data.runDeccelAmount = c_runDeccelAmount;
        Data.accelInAir = c_accelInAir;
        Data.deccelInAir = c_deccelInAir;
        Data.jumpHangAccelerationMult = c_jumpHangAccelerationMult;
        Data.jumpHangMaxSpeedMult = c_jumpHangMaxSpeedMult;
        Data.doConserveMomentum = c_doConserveMomentum;
        Data.jumpHangTimeThreshold = c_jumpHangTimeThreshold;
    }

    #region INPUT CALLBACKS
    private void OnMoveInput(InputAction.CallbackContext ctx)
    {
        _currentMoveInput = ctx.ReadValue<Vector2>(); // cache input

        if (_currentMoveInput.x != 0)
            CheckDirectionToFace(_currentMoveInput.x > 0);

    }
    public void OnJumpUpInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            LastPressedJumpTime = Data.jumpInputBufferTime;
        }
        else if (ctx.canceled && CanJumpCut())
        {
            _isJumpCut = true;
        }
    }
    #endregion

    private void Update()
    {
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        #endregion

        #region INPUT HANDLER
        /*        _moveInput.x = Input.GetAxisRaw("Horizontal");

                if (_moveInput.x != 0)
                    CheckDirectionToFace(_moveInput.x > 0);*/
        #endregion

        #region COLLISION CHECKS
        //Ground Check
        bool isGrounded = Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer);

        if (isGrounded)
        {
            LastOnGroundTime = IsJumping ? 0.1f : Data.coyoteTime;

            _jumpsLeft = Data.totalJumps;
        }
        #endregion

        #region JUMP CHECKS
        if (IsJumping && RB.linearVelocity.y < 0)
        {
            IsJumping = false;

            _isJumpFalling = true;
        }

        if (LastOnGroundTime > 0 && !IsJumping)
        {
            //_isJumpCut = false;

            if (!IsJumping)
            {
                _isJumpFalling = false;
                _isJumpCut= true;
            }
        }

        //Jump
        if (CanJump() && LastPressedJumpTime > 0)
        {
            IsJumping = true;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump();
        }
        #endregion

        #region GRAVITY
        //Higher gravity if we've released the jump input or are falling
        if (IsSliding)
        {
            SetGravityScale(0);
        }
        else if (RB.linearVelocity.y < 0 && _currentMoveInput.y < 0)
        {
            //Much higher gravity if holding down
            SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFastFallSpeed));
        }
        else if (_isJumpCut)
        {
            //Higher gravity if jump button released
            SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
            RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFallSpeed));
        }
        else if ((IsJumping || _isJumpFalling) && Mathf.Abs(RB.linearVelocity.y) < Data.jumpHangTimeThreshold)
        {
            SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
        }
        else if (RB.linearVelocity.y < 0)
        {
            //Higher gravity if falling
            SetGravityScale(Data.gravityScale * Data.fallGravityMult);
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFallSpeed));
        }
        else
        {
            //Default gravity if standing on a platform or moving upwards
            SetGravityScale(Data.gravityScale);
        }
        #endregion
    }

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }
    #endregion

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.75f, 0.0f, 0.0f, 0.75f);
        Gizmos.DrawCube(_groundCheckPoint.position, _groundCheckSize);
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    //MOVEMENT METHODS
    #region RUN METHODS
    private void ApplyMovement()
    {
        float inputX = _currentMoveInput.x;

        float targetSpeed = inputX * Data.runMaxSpeed;
        float accelRate;
        #region Calculate AccelRate
        if (LastOnGroundTime > 0)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        }
        #endregion

        //Not used since no jump implemented here, but may be useful if you plan to implement your own

        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((IsJumping || _isJumpFalling) && Mathf.Abs(RB.linearVelocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion


        #region Conserve Momentum
        if (Data.doConserveMomentum && Mathf.Abs(RB.linearVelocity.x) > Mathf.Abs(targetSpeed)
            && Mathf.Sign(RB.linearVelocity.x) == Mathf.Sign(targetSpeed)
            && Mathf.Abs(targetSpeed) > 0.01f
            && LastOnGroundTime < 0)
        {
            accelRate = 0;
        }
        #endregion

        float speedDif = targetSpeed - RB.linearVelocity.x;
        float movement = speedDif * accelRate;

        if (m_animator != null)
        {
            float horizontalVelocity = Mathf.Abs(RB.linearVelocity.x);
            bool isWalking = horizontalVelocity > 0.1f && LastOnGroundTime > 0;
            m_animator.SetBool("IsWalking", isWalking);
        }

        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Run(InputAction.CallbackContext ctx)
    {

        Vector2 inputValue = ctx.ReadValue<Vector2>();
        inputValue = Vector2.ClampMagnitude(inputValue, 1f);

        Debug.Log($"Joystick Input: {inputValue} (Magnitude: {inputValue.magnitude})");

        CheckDirectionToFace(inputValue.x > 0);

        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = inputValue.x * Data.runMaxSpeed;
        print(targetSpeed);
        #region Calculate AccelRate
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (LastOnGroundTime > 0) //we are landed
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
            print("LastOnGroundTime > 0");
            print(accelRate);
        }
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        //Not used since no jump implemented here, but may be useful if you plan to implement your own
        /* 
		#region Add Bonus Jump Apex Acceleration
		//Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
		if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
		{
			accelRate *= Data.jumpHangAccelerationMult;
			targetSpeed *= Data.jumpHangMaxSpeedMult;
		}
		#endregion
		*/

        #region Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (Data.doConserveMomentum && Mathf.Abs(RB.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - RB.linearVelocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
    }

    private void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        //Ensures we can't call Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        _jumpsLeft--;

        #region Perform Jump
        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount 
        //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
        float force = Data.jumpForce;
        if (RB.linearVelocity.y < 0)
            force -= RB.linearVelocity.y;

        float adjustedForce = force * RB.mass;

        RB.AddForce(Vector2.up * adjustedForce, ForceMode2D.Impulse);
        #endregion
    }
    #endregion

    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }
    private bool CanJump()
    {
        return _jumpsLeft > 0;
        //return LastOnGroundTime > 0 && !IsJumping;
    }
    private bool CanJumpCut()
    {
        return IsJumping && RB.linearVelocity.y > 0;
    }
    #endregion
}

