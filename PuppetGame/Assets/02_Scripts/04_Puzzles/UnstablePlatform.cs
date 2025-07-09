using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PassRequirements))]
public class UnstablePlatform : MonoBehaviour
{
    PassRequirements m_PassRequirements;

    private void Start()
    {
        m_PassRequirements= gameObject.GetComponent<PassRequirements>(); 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ability currCharacterAbility = collision.gameObject.GetComponent<Ability>();
        
        if (currCharacterAbility)
        {
            if (!m_PassRequirements.CanCharacterPass(currCharacterAbility))
                BreakPlatform();
        }
    }

    void BreakPlatform()
    {
        Debug.Log("Charater cant pass");
    }
}
