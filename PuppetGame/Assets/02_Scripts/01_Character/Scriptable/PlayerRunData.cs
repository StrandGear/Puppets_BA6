using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerRunData", menuName = "Scriptable Objects/PlayerRunData")]
public class PlayerRunData : ScriptableObject
{
    [Header("Gravity")]
    [HideInInspector] public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
    [HideInInspector] public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
                                                 //Also the value the player's rigidbody2D.gravityScale is set to.
    [Space(5)]
    [Tooltip("If this is too low, falling is slow floaty landing, if too high, fall like a brick.")]
    public float fallGravityMult; //Multiplier to the player's gravityScale when falling. 
                                  //If this is too low, falling will be slow floaty landing.
                                  //If it’s too high, you fall like a brick.

    [Tooltip("Activated when player holds down during fall.")]
    public float maxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.
    //Activated when player holds down during fall
    //If you're not planning to implement fast fall yet, set to 1 

    [Space(5)]
    [Tooltip("Activated when player holds down during fall.")]
    public float fastFallGravityMult; //Larger multiplier to the player's gravityScale when they are falling and a downwards input is pressed.
                                      //Seen in games such as Celeste, lets the player fall extra fast if they wish.
    [Tooltip("Activated when player holds down during fall.")]
    public float maxFastFallSpeed; //Maximum fall speed(terminal velocity) of the player when performing a faster fall.

    [Space(20)]

    [Header("Run")]
    public float runMaxSpeed; //Target speed we want the player to reach.
    [Tooltip("Time we want it to take for the player to accelerate from 0 to the runMaxSpeed.")]
    public float runAcceleration; //Time (approx.) time we want it to take for the player to accelerate from 0 to the runMaxSpeed.
    [HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.

    [Tooltip("Time we want it to take for the player to accelerate from runMaxSpeed to 0.")]
    public float runDecceleration; //Time (approx.) we want it to take for the player to accelerate from runMaxSpeed to 0.

    [HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
    [Space(10)]
    [Header("Multipliers applied to acceleration rate when airborne")]
    [Range(0.01f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
    [Range(0.01f, 1)] public float deccelInAir;
    public bool doConserveMomentum;

    [Space(20)]

    [Header("Jump")]
    [Tooltip("How high player jumps.")]
    public float jumpHeight; //Height of the player's jump
    [Tooltip("How fast player reaches that height.")]
    public float jumpTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
    [HideInInspector] public float jumpForce; //The actual force applied (upwards) to the player when they jump.

    [Header("Both Jumps")]
    [Tooltip("Multiplier to increase gravity if the player releases thje jump button while still jumping.")]
    public float jumpCutGravityMult; //Multiplier to increase gravity if the player releases thje jump button while still jumping

    [Tooltip("Simulates a short hover at the top of the jump.  0.3–0.6 floaty and responsive.")]
    [Range(0f, 1)] public float jumpHangGravityMult; //Reduces gravity while close to the apex (desired max height) of the jump 
    //simulates a short hover at the top of the jump 
    //If this value is too large and gravity mult too low = player floats at apex too long

    [Tooltip("Sort of how long they float at the apex, best time to make a second jump.")]
    public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
    [Space(0.5f)]
    
    [Tooltip("These make mid-air movement snappier only during the hang phase. Leave at 1 if no need in air control.")]
    public float jumpHangAccelerationMult;
    [Tooltip("These make mid-air movement snappier only during the hang phase. Leave at 1 if no need in air control.")]
    public float jumpHangMaxSpeedMult;
    [Space(10)]
    public int totalJumps = 1;

    [Space(20)]

    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

    /*
    If jump feels too slow:
    -Lower jumpTimeToApex
    -Increase fallGravityMult

    If jump feels floaty at top:
    -Decrease jumpHangGravityMult
    -Lower jumpHangTimeThreshold 
     */

    private void OnValidate()
    {
        //Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        //Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
        gravityScale = gravityStrength / Physics2D.gravity.y;

        //Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        //Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        #region Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
    }

}
