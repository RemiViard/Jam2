using UnityEngine;
using UnityEngine.Events;

public class FishMvtBehavior : MonoBehaviour
{
    private Transform m_transform;

    public float TimeBeforeChangingDirection = 3f;
    public bool isAlive;
    [SerializeField] float m_Speed;
    [SerializeField] int health;

    private Rigidbody2D m_Rigidbody;
    private float m_currentTimer;

    UnityEvent onDeath = new UnityEvent();

    void Start()
    {
        isAlive = true;
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_currentTimer = TimeBeforeChangingDirection;
        m_transform = GetComponent<Transform>();

        // Binding Events
        onDeath.AddListener(OnDeath);
    }

    void Update()
    {
        if (health <= 0)
        {
            onDeath.Invoke();
            isAlive = false;
        }
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
        Vector2 rot = Random.insideUnitCircle.normalized;
        m_transform.LookAt(m_transform.position + new Vector3(rot.x, rot.y, 0));
    }

    private void Move()
    {
        m_Rigidbody.linearVelocity = m_transform.forward * m_Speed;
    }

    // Events
    private void OnDeath()
    {
        // VFX OU SFX de la mort qui tue (litteralement)
    }
}
