using UnityEngine;

public class FishMvtBehavior : MonoBehaviour
{
    private Transform m_transform;

    public float TimeBeforeChangingDirection = 3f;
    public bool isAlive;
    public float m_Speed;

    private Rigidbody2D m_Rigidbody;

    private float m_currentTimer;
    void Start()
    {
        isAlive = true;
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_currentTimer = TimeBeforeChangingDirection;
        m_transform = GetComponent<Transform>();
    }

    void Update()
    {
        if (isAlive)
        {
            Move();
            m_currentTimer -= Time.deltaTime;
            if (m_currentTimer <= 0)
            {
                ChangeDirection();
                m_currentTimer = TimeBeforeChangingDirection;
            }
        }
    }

    private void ChangeDirection()
    {
        Vector3 rot = Random.insideUnitCircle;
        m_transform.Rotate(rot.x, rot.y, rot.z);
    }

    private void Move()
    {
        m_Rigidbody.linearVelocity = m_transform.forward * m_Speed;
    }
}
