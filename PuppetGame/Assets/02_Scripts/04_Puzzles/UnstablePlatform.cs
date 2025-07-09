using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PassRequirements))]
public class UnstablePlatform : MonoBehaviour
{
    PassRequirements m_PassRequirements;

    [Header("Animation Parameters")]
    [SerializeField] private float m_animLoopDuration = 0.5f;
    [SerializeField] private float m_offsetToMove = 0.02f;

    [Header("Platform Braking Parts")]
    [SerializeField] float m_secondsBeforeBreaks = 0.2f;
    [SerializeField] GameObject m_leftPart;
    [SerializeField] GameObject m_rightPart;

    private Timer m_breakTimer;

    private void Start()
    {
        m_PassRequirements= gameObject.GetComponent<PassRequirements>();
        /*
                // Tiny rotation around Z
                LeanTween.rotateZ(gameObject, 0.8f, 0.4f)
                    .setEase(LeanTweenType.easeShake)
                    .setLoopPingPong(-1);*/

        /*        
                 // Randomized wiggle
                LeanTween.moveLocal(gameObject, transform.localPosition + new Vector3(0.02f, 0.02f), 0.4f)
            .setEase(LeanTweenType.easeShake)
            .setLoopPingPong(-1);*/

        LeanTween.moveLocalX(gameObject, transform.localPosition.x + m_offsetToMove, m_animLoopDuration)
    .setEase(LeanTweenType.easeShake)
    .setLoopPingPong(-1);  // -1 means infinite loop

    }

    private void Update()
    {
        m_breakTimer?.Update();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ability currCharacterAbility = collision.gameObject.GetComponent<Ability>();
        
        if (currCharacterAbility)
        {
            if (!m_PassRequirements.CanCharacterPass(currCharacterAbility))
            {
                //BreakPlatform();

                if (m_breakTimer == null || !m_breakTimer.IsRunning)
                {
                    m_breakTimer = new Timer(m_secondsBeforeBreaks, BreakPlatform);
                    m_breakTimer.Start();

                    LeanTween.cancel(gameObject); // stop old tween

                    LeanTween.moveLocalX(gameObject, transform.localPosition.x + 0.1f, 0.1f)
                        .setEase(LeanTweenType.easeShake)
                        .setLoopPingPong(-1);

                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Ability currCharacterAbility = collision.gameObject.GetComponent<Ability>();

        if (currCharacterAbility)
        {
            // stop the timer so it doesn't break
            m_breakTimer?.Stop();
            m_breakTimer = null;

            // return to gentle shake
            LeanTween.cancel(gameObject);
            LeanTween.moveLocalX(gameObject, transform.localPosition.x + 0.02f, 0.5f)
                .setEase(LeanTweenType.easeShake)
                .setLoopPingPong(-1);
        }
    }


    void BreakPlatform()
    {
        LeanTween.cancel(gameObject);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
        m_leftPart.SetActive(true);
        m_rightPart.SetActive(true);
    }
}
