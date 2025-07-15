using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    //[HideInInspector] public Action<bool, bool> OnMovementSateChanged;
    [HideInInspector] public Action OnAbilitStateChanged;

    bool m_currentlyMoving = false;
    bool m_currentlyJumping = false;

    [SerializeField] string m_abilityStateOn, m_abilityStateOff;
    [SerializeField] bool m_abilityOn = false;

    Animator m_animator;

    private void Awake()
    {
        if (!gameObject.GetComponent<CharacterMovement>())
            this.enabled= false;

        m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
       // OnMovementSateChanged += UpdateMovementState;
        OnAbilitStateChanged += SwitchAbilityState;

        SwitchAbilityState();
    }

    public void UpdateMovementState(bool walking)
    {
        //print("Movement Animation is changing");

        if (walking)
        {
            if (m_currentlyMoving)
                return;

            m_currentlyMoving = true;
            m_animator.SetFloat("Speed", 1f);
            m_animator.SetBool("IsIdle", false);
        }
        else
        {
            m_currentlyMoving = false;
            m_animator.SetBool("IsIdle", true);
            m_animator.SetFloat("Speed", 0.0f);
        }
    }

    public void Jump()
    {
        print("In Jump anim method");
        m_animator.SetBool("IsJumping", true);
        m_currentlyJumping = true;
    }
    public void SetGroundedState()
    {
        print("Grounded");
        m_currentlyJumping = false;
        m_animator.SetBool("IsJumping", false);
    }


    void SwitchAbilityState() //assume each has 2 states for now //TODO: assign in inspector or count states from Ability component
    {
        m_abilityOn = !m_abilityOn;

        if (m_abilityOn)
        {
            m_animator.SetBool(m_abilityStateOff, false);
            m_animator.SetBool(m_abilityStateOn, true);
        }
        else
        {
            m_animator.SetBool(m_abilityStateOn, false);
            m_animator.SetBool(m_abilityStateOff, true);
        }
    }
}
