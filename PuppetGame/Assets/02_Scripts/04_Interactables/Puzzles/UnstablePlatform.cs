using UnityEngine;
using System.Collections.Generic;

public class UnstablePlatform : Puzzle
{

    [Header("Animation Parameters")]
    [SerializeField] private float m_animLoopDuration = 0.5f;
    [SerializeField] private float m_offsetToMove = 0.02f;

    [Header("Platform Braking Parts")]
    [SerializeField] float m_secondsBeforeBreaks = 0.2f;
    [SerializeField] GameObject m_leftPart;
    [SerializeField] GameObject m_rightPart;

    private Timer m_breakTimer;

    protected override void Start()
    {
        base.Start();
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

    protected override void CharacterEnteredWithoutPassedRequirment()
    {
        if (m_breakTimer == null || !m_breakTimer.IsRunning)
        {
            m_breakTimer = new Timer(m_secondsBeforeBreaks, BreakPlatform);
            m_breakTimer.Start();

            LeanTween.cancel(gameObject);
            LeanTween.moveLocalX(gameObject, transform.localPosition.x + 0.1f, 0.1f)
                .setEase(LeanTweenType.easeShake)
                .setLoopPingPong(-1);
        }
    }

    protected override void CharacterExited()
    {
        m_breakTimer?.Stop();
        m_breakTimer = null;

        LeanTween.cancel(gameObject);
        LeanTween.moveLocalX(gameObject, transform.localPosition.x + 0.02f, 0.5f)
            .setEase(LeanTweenType.easeShake)
            .setLoopPingPong(-1);
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
