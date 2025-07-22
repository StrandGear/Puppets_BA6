using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterMovement))]
[AddComponentMenu("Character/Abilities/AdjustTension")]
public class AdjustTension : Ability
{
    [SerializeField] List<Transform> m_ropesAnchors = new List<Transform>();
    [SerializeField] float m_moveY = 5f;
    [SerializeField] float m_speed = 2f;

    private Vector3[] m_startPositions;
    private Vector3[] m_targetPositions;

    private bool m_moving = false;
    [Tooltip("Initial state of the rope, isUp = tight state.")]
    private bool m_isUp = true;
    private float m_lerpTime = 0f;

    float m_initialMaxSpeed;

    CharacterMovement m_movement;

    public PlayerRunData m_tightMovementData;
    PlayerRunData m_loseMovementData;

    // [Range(0.01f, 1)] [SerializeField] float m_speedReduction = 0.3f;

    // float m_initMass;
    //  [SerializeField] float m_biggerMass = 3.0f;

    InputAction m_waveButton;

    public StringPuppetStates currentAbilityState { get; private set; }

    private void Awake()
    {
        m_movement = GetComponent<CharacterMovement>();
        m_loseMovementData = m_movement.Data;
    }

    protected override void Start()
    {
        base.Start();

        characterType = CharacterType.StringPuppet;

        m_abilityButton.Enable();

        m_abilityButton = InputSystem.actions.FindAction("Interact");
        m_abilityButton.performed += UseAbility;
        m_abilityButton.canceled += UseAbility;

        m_waveButton = InputSystem.actions.FindAction("Crouch");
        m_waveButton.performed += Wave;
        m_waveButton.canceled += Wave;

        // starting positions
        m_startPositions = new Vector3[m_ropesAnchors.Count];
        for (int i = 0; i < m_ropesAnchors.Count; i++)
        {
            m_startPositions[i] = m_ropesAnchors[i].position;
        }

        // target is same as start
        m_targetPositions = new Vector3[m_ropesAnchors.Count];
        System.Array.Copy(m_startPositions, m_targetPositions, m_startPositions.Length);

        
        //m_initialMaxSpeed = m_movement.Data.runMaxSpeed;
        //m_initMass = GetComponent<Rigidbody2D>().mass;

        //initil state of the rope is tight so we adjust the speed in the beginning to slower 
        ToggleCharacterMovement();
    }

    private void Update()
    {
        if (m_moving)
        {
            m_lerpTime += Time.deltaTime * m_speed;
            float t = Mathf.Clamp01(m_lerpTime);

            for (int i = 0; i < m_ropesAnchors.Count; i++)
            {
                m_ropesAnchors[i].position = Vector3.Lerp(
                    m_startPositions[i],
                    m_targetPositions[i],
                    t
                );
            }

            if (t >= 1f)
            {
                m_moving = false;

                for (int i = 0; i < m_ropesAnchors.Count; i++)
                {
                    m_startPositions[i] = m_ropesAnchors[i].position;
                }

                m_isUp = !m_isUp;

                ToggleCharacterMovement();
            }

        }
    }

    public override void UseAbility(InputAction.CallbackContext obj)
    {
        base.UseAbility(obj);

        ToggleRopeMovement();

        GetComponent<CharacterAnimationController>().OnAbilitStateChanged.Invoke();
    }
     void Wave (InputAction.CallbackContext obj)
    {
        print("Crouching action ");
        GetComponent<Animator>().SetBool("IsWaving", true);
    }

    private void ToggleRopeMovement()
    {
        if (m_moving) return; // prevent mid-move toggle

        // Set new target positions
        for (int i = 0; i < m_ropesAnchors.Count; i++)
        {
            Vector3 pos = m_ropesAnchors[i].position;
            if (!m_isUp)
                m_targetPositions[i] = new Vector3(pos.x, m_startPositions[i].y + m_moveY, pos.z);
            else
                m_targetPositions[i] = new Vector3(pos.x, m_startPositions[i].y - m_moveY, pos.z);
        }

        m_lerpTime = 0f;
        m_moving = true;
    }

    private void ToggleCharacterMovement()
    {
        if (m_isUp)
        {
            m_movement.Data = m_tightMovementData;
            //high tension -> slower percise movement -> 
            //GetComponent<CharacterMovement>().Data.runMaxSpeed *= m_speedReduction;
            //GetComponent<Rigidbody2D>().gravityScale = 5;
            //GetComponent<Rigidbody2D>().mass = m_biggerMass;
            //currentAbilityState = StringPuppetStates.HighTension;
        }
        else
        {
            m_movement.Data = m_loseMovementData;
            //GetComponent<CharacterMovement>().Data.runMaxSpeed = m_initialMaxSpeed;
            //GetComponent<Rigidbody2D>().gravityScale = 1;
            //GetComponent<Rigidbody2D>().mass = m_initMass;
            //currentAbilityState = StringPuppetStates.LowTension;
        }
    }
}
