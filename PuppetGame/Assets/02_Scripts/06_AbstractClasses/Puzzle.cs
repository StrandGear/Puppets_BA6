using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PassRequirements))]
public abstract class Puzzle : MonoBehaviour
{
    PassRequirements m_PassRequirements;
    protected PassRequirements PassRequirements
    {
        get
        {
            if (m_PassRequirements == null)
                m_PassRequirements = GetComponent<PassRequirements>();
            return m_PassRequirements;
        }
    }

    private void OnEnable()
    {
        gameObject.layer = 6;
    }

    protected virtual void Start()
    {
        m_PassRequirements= GetComponent<PassRequirements>();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
/*        Ability currCharacterAbility = collision.gameObject.GetComponent<Ability>();

        if (currCharacterAbility)
        {
            if (PassRequirements.CanCharacterPass(currCharacterAbility))
            {
                CharacterEnteredWithPassedRequirment();
            }
            else
            {
                Debug.Log(" 11111 Character does NOT pass requirements - moving DOWN");
                CharacterEnteredWithoutPassedRequirment();
            } 
                
        }*/
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        Ability currCharacterAbility = collision.gameObject.GetComponent<Ability>();

        if (currCharacterAbility)
        {
            Debug.Log("Character exited - moving to INIT");
            CharacterExited();
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        Ability currCharacterAbility = collision.gameObject.GetComponent<Ability>();

        if (currCharacterAbility)
        {
            if (PassRequirements.CanCharacterPass(currCharacterAbility))
            {
                CharacterEnteredWithPassedRequirment();
            }
            else
            {
                Debug.Log("2222 Character does NOT pass requirements - moving DOWN");
                CharacterEnteredWithoutPassedRequirment();
            }

        }
    }

    protected virtual void CharacterEnteredWithPassedRequirment() {}
    protected virtual void CharacterEnteredWithoutPassedRequirment() { }
    protected virtual void CharacterExited() { }
}
