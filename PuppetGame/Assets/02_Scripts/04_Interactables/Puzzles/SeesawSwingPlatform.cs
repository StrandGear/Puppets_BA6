using UnityEngine;
/*
public class SeesawSwingPlatform : Puzzle
{
    [SerializeField] int m_rotationAngle = 45;
    [SerializeField] private float m_rotationTime = 0.5f;
    [SerializeField] WeightSide m_weightSide = WeightSide.Left;

    private float m_targetAngle;

    Timer m_positionResetTimer;

    private void Awake()
    {
        m_positionResetTimer = new Timer(0.4f, ResetPlatformLean);
    }

    protected override void Start()
    {
        base.Start();

        if (m_weightSide == WeightSide.Left) { LeanLeft(); }
        else LeanRight();
    }
    private void Update()
    {
        m_positionResetTimer?.Update();
    }

    protected override void CharacterEnteredWithoutPassedRequirment()
    {
        // sharacter too heavy - lean towards them
        m_positionResetTimer.Stop();

        if (m_weightSide == WeightSide.Left)
            LeanRight();
        else
            LeanLeft();
    }

    protected override void CharacterEnteredWithPassedRequirment()
    {
        m_positionResetTimer.Stop();

        if (m_weightSide == WeightSide.Left)
            LeanLeft();
        else
            LeanRight();
    }
    protected override void CharacterExited()
    {
        if (m_weightSide == WeightSide.Left)
            LeanLeft();
        else
            LeanRight();

        m_positionResetTimer.Start();
    }

    private void LeanLeft()
    {
        m_targetAngle = m_rotationAngle;

        LeanTween.cancel(gameObject);
        LeanTween.rotateZ(gameObject, m_targetAngle, m_rotationTime).setEase(LeanTweenType.easeInOutSine);

    }

    private void LeanRight()
    {
        m_targetAngle = -m_rotationAngle;

        LeanTween.cancel(gameObject);
        LeanTween.rotateZ(gameObject, m_targetAngle, m_rotationTime).setEase(LeanTweenType.easeInOutSine);

    }

    private void ResetPlatformLean()
    {
        if (m_weightSide == WeightSide.Left)
            LeanLeft();
        else
            LeanRight();
    }

}

public enum WeightSide
{
    Left,
    Right
}*/


public class SeesawSwingPlatform : Puzzle
{
    [SerializeField] Rigidbody2D m_weightedObject;

    [SerializeField] Rigidbody2D m_playerRB;
    float m_initPlayerGravityValue;

    protected override void Start()
    {
        base.Start();

        m_initPlayerGravityValue = m_playerRB.gravityScale;
    }

    protected override void CharacterEnteredWithoutPassedRequirment()
    {
        m_weightedObject.mass = 80.0f;
        m_playerRB.gravityScale = m_initPlayerGravityValue;
    }

    protected override void CharacterEnteredWithPassedRequirment()
    {
        m_weightedObject.mass = 1.0f;
        m_playerRB.gravityScale = 5.0f;
    }
    protected override void CharacterExited()
    {
        m_weightedObject.mass = 80.0f;
        m_playerRB.gravityScale = m_initPlayerGravityValue;
    }
}