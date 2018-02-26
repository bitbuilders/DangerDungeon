using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature, IMovable
{
    [SerializeField] public GameObject m_foe = null;
    [SerializeField] Transform m_groundTouch = null;
    [SerializeField] Transform m_minPatrol = null;
    [SerializeField] Transform m_maxPatrol = null;
    [SerializeField] GameObject m_weaponHitbox = null;
    [SerializeField] [Range(1.0f, 100.0f)] float m_health = 100.0f;
    [SerializeField] [Range(1.0f, 5000.0f)] float m_speed = 1500.0f;
    [SerializeField] [Range(1.0f, 20.0f)] float m_jumpForce = 17.0f;
    [SerializeField] [Range(0.5f, 5.0f)] float m_patrolPause = 1.0f;
    [SerializeField] [Range(0.0f, 3.0f)] float m_pauseVariation = 0.5f;
    //[SerializeField] bool m_patrol = true;
    [SerializeField] [Range(1.0f, 10.0f)] float m_detectionRadius = 5.0f;
    [SerializeField] [Range(1.0f, 180.0f)] float m_fov = 40.0f;
    [SerializeField] [Range(1.0f, 5.0f)] float m_attackRadius = 1.0f;
    [SerializeField] [Range(0.1f, 3.0f)] float m_attackCooldown = 0.5f;
    [SerializeField] [Range(1.0f, 50.0f)] float m_attackDamage = 10.0f;
    [SerializeField] LayerMask m_groundMask;
    [SerializeField] LayerMask m_platformMask;

    Rigidbody2D m_rigidbody2D;
    Animator m_animator;
    SpriteRenderer m_spriteRenderer;
    StackStateMachine<Enemy> m_stateMachine;
    Vector3 m_startScale;
    Vector3 m_currentScale;
    Vector2 m_force = Vector2.zero;
    float m_attackReset = 0.0f;
    float m_attackTime = 0.0f;
    float m_speedMultiplier = 1.0f;
    float m_pauseDuration = 0.0f;
    float m_pauseTime = 0.0f;
    bool m_movingRight = true;
    bool m_paused = false;
    bool m_onGround = true;
    bool m_alerted = false;

    public float SpeedMultiplier { get { return m_speedMultiplier; } set { m_speedMultiplier = value; } }
    public float AttackDamage { get { return m_attackDamage; } }
    public bool IsAlive { get { return m_health > 0.0f; } }

    void Start()
    {
        m_stateMachine = new StackStateMachine<Enemy>();
        m_stateMachine.AddState("Patrol", new PatrolState<Enemy>(this));
        m_stateMachine.AddState("Attack", new AttackState<Enemy>(this));
        m_stateMachine.AddState("Seek", new SeekState<Enemy>(this));
        m_stateMachine.AddState("Jump", new JumpState<Enemy>(this));
        m_stateMachine.AddState("Drop", new DropState<Enemy>(this));
        m_stateMachine.PushState("Patrol");

        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        m_startScale = transform.localScale;
        m_currentScale = m_startScale;

        m_attackReset = Random.Range(m_attackCooldown, m_attackCooldown + 0.25f);
    }

    void Update()
    {
        m_onGround = IsOnGround(m_groundTouch.position, m_groundMask, m_platformMask, 0.2f);
        m_animator.SetBool("OnGround", m_onGround);

        if (IsAlive)
        {
            m_stateMachine.Update();
        }
    }

    private void FixedUpdate()
    {
        if (IsAlive)
        {
            m_force = m_force * Time.deltaTime;
            m_force = (m_onGround) ? m_force : m_force * 0.5f;
            m_rigidbody2D.AddForce(m_force);

            if (m_onGround)
            {
                m_animator.SetFloat("Walk", m_rigidbody2D.velocity.magnitude);
                m_animator.SetFloat("WalkSpeed", m_rigidbody2D.velocity.magnitude * 0.25f);
            }
            else
            {
                m_animator.SetFloat("Walk", 0.0f);
                m_animator.SetFloat("WalkSpeed", 0.0f);
            }

            m_animator.SetFloat("yVelocity", m_rigidbody2D.velocity.y);
        }

        if (m_rigidbody2D.velocity.y > 0.0f && !m_onGround)
        {
            m_rigidbody2D.velocity += ((Vector2.up * Physics2D.gravity.y)) * (m_jumpResistance - 1.0f) * Time.deltaTime;
        }
        else if (m_rigidbody2D.velocity.y < 0.0f && !m_onGround)
        {
            m_rigidbody2D.velocity += ((Vector2.up * Physics2D.gravity.y)) * (m_fallMultiplier - 1.0f) * Time.deltaTime;
        }
    }

    public void HideWeaponHitbox()
    {
        m_weaponHitbox.SetActive(false);
    }

    void ShowWeaponHitbox()
    {
        m_weaponHitbox.SetActive(true);
    }

    void StartPauseTimer()
    {
        m_pauseDuration = m_patrolPause + Random.Range(-1.0f, 1.0f) * m_pauseVariation;
        m_paused = true;
        m_pauseTime = 0.0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            if (Mathf.Abs(m_maxPatrol.position.y - transform.position.y) > 1.5f)
            {
                Vector2 position = m_minPatrol.position;
                position.y = transform.position.y;
                m_minPatrol.position = position;

                Vector2 position2 = m_maxPatrol.position;
                position2.y = transform.position.y;
                m_maxPatrol.position = position2;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponentInParent<Player>();
        if (player && collision.CompareTag("FriendlyWeapon"))
        {
            KnockBack((transform.position - player.transform.position).normalized * 4.0f);
            TakeDamge(player.AttackDamage);

            float angle = GetAngleFromEnemy();
            if (angle > 160.0f)
            {
                m_alerted = true;
            }

            player.HideWeaponHitbox();
        }
        else if (collision.CompareTag("Bullet"))
        {
            KnockBack((transform.position - collision.transform.position).normalized * 4.0f);
            TakeDamge(m_foe.GetComponent<Player>().AttackDamage);

            float angle = GetAngleFromEnemy();
            if (angle > 160.0f)
            {
                m_alerted = true;
            }

            collision.gameObject.SetActive(false);
        }
    }

    private void TakeDamge(float amount)
    {
        if (m_health > 0.0f)
        {
            m_health -= amount;

            if (m_health <= 0.0f)
            {
                Die();
            }
            else
            {
                AudioManager.Instance.PlayClip("EnemyGrunt");
            }
        }
    }

    private void Die()
    {
        m_animator.SetTrigger("Die");
        m_animator.SetBool("IsAlive", IsAlive);

        GetComponent<Collider2D>().enabled = false;
        m_rigidbody2D.velocity = Vector2.zero;
        
        float angle = Random.Range(-45.0f, 45.0f);
        Vector2 force = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up;
        Vector3 pos = force;
        Debug.DrawLine(transform.position, transform.position + pos * 10.0f, Color.green, 1.0f);
        KnockBack(force * 15.0f);

        AudioManager.Instance.PlayClip("EnemyDie");

        StartCoroutine(FadeCorpse());
        StartCoroutine(SpinCorpse());
    }

    IEnumerator FadeCorpse()
    {
        yield return new WaitForSeconds(1.0f);
        
        for (float i = 1.0f; i >= 0.0f; i -= Time.deltaTime / 2.0f)
        {
            Color c = m_spriteRenderer.color;
            c.a = i;
            m_spriteRenderer.color = c;

            yield return null;
        }

        Destroy(gameObject);
    }

    IEnumerator SpinCorpse()
    {
        float spinForce = Random.Range(180.0f, 720.0f);

        for (float i = 2.0f; i >= 0.0f; i -= Time.deltaTime)
        {
            transform.rotation *= Quaternion.Euler(Vector3.forward * spinForce * Time.deltaTime);

            yield return null;
        }
    }

    private float GetAngleFromEnemy()
    {
        Vector2 direction = m_foe.transform.position - transform.position;
        Vector2 lookDirection = (transform.localScale.x < 0.0f) ? Vector2.left : Vector2.right;
        float angle = Vector2.Angle(direction.normalized, lookDirection);

        return angle;
    }

    private void KnockBack(Vector2 force)
    {
        m_rigidbody2D.AddForce(force, ForceMode2D.Impulse);
    }

    class PatrolState<T> : State<T> where T : Enemy
    {
        public PatrolState(T owner) : base(owner) { }

        public override void Enter()
        {
        }

        public override void Update()
        {
            if (m_owner.transform.position.x < m_owner.m_minPatrol.position.x && m_owner.m_movingRight == false && !m_owner.m_paused)
            {
                m_owner.m_movingRight = true;
                m_owner.StartPauseTimer();
            }
            else if (m_owner.transform.position.x > m_owner.m_maxPatrol.position.x && m_owner.m_movingRight == true && !m_owner.m_paused)
            {
                m_owner.m_movingRight = false;
                m_owner.StartPauseTimer();
            }

            if (m_owner.m_paused)
            {
                m_owner.m_pauseTime += Time.deltaTime;
                if (m_owner.m_pauseTime >= m_owner.m_pauseDuration)
                {
                    m_owner.m_paused = false;
                    m_owner.m_pauseTime = 0.0f;
                }
            }

            Vector2 direction = m_owner.m_foe.transform.position - m_owner.transform.position;
            float angle = m_owner.GetAngleFromEnemy();
            if ((angle < m_owner.m_fov * 2.0f || m_owner.m_alerted) && direction.magnitude <= m_owner.m_attackRadius)
            {
                m_owner.m_stateMachine.PushState("Attack");
            }
            else if ((angle < m_owner.m_fov || m_owner.m_alerted) && direction.magnitude <= m_owner.m_detectionRadius)
            {
                m_owner.m_stateMachine.PushState("Seek");
            }
            else
            {
                m_owner.m_alerted = false;

                Vector2 walkDirection = m_owner.m_maxPatrol.transform.position + (Vector3.right * 0.5f) - m_owner.transform.position;
                if (m_owner.m_movingRight && !m_owner.m_paused)
                {
                    m_owner.m_force = walkDirection.normalized * m_owner.m_speed * m_owner.m_speedMultiplier;
                    m_owner.m_currentScale = m_owner.Flip(walkDirection, m_owner.m_startScale, m_owner.m_currentScale);
                    m_owner.transform.localScale = m_owner.m_currentScale;
                }
                else if (!m_owner.m_movingRight && !m_owner.m_paused)
                {
                    walkDirection = m_owner.m_minPatrol.transform.position + (Vector3.left * 0.5f) - m_owner.transform.position;
                    m_owner.m_force = walkDirection.normalized * m_owner.m_speed * m_owner.m_speedMultiplier;
                    m_owner.m_currentScale = m_owner.Flip(walkDirection, m_owner.m_startScale, m_owner.m_currentScale);
                    m_owner.transform.localScale = m_owner.m_currentScale;
                }
            }
        }

        public override void Exit()
        {
        }
    }

    class AttackState<T> : State<T> where T : Enemy
    {
        public AttackState(T owner) : base(owner) { }

        public override void Enter()
        {
        }

        public override void Update()
        {
            m_owner.m_attackTime += Time.deltaTime;
            if (!m_owner.m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && m_owner.m_attackTime >= m_owner.m_attackReset)
            {
                m_owner.m_animator.SetTrigger("Attack");
                m_owner.m_attackTime = 0.0f;
                m_owner.m_attackReset = Random.Range(m_owner.m_attackCooldown, m_owner.m_attackCooldown + 0.25f);
            }

            Vector2 distance = m_owner.m_foe.transform.position - m_owner.transform.position;
            m_owner.m_currentScale = m_owner.Flip(distance, m_owner.m_startScale, m_owner.m_currentScale);
            m_owner.transform.localScale = m_owner.m_currentScale;
            if (distance.magnitude >= m_owner.m_attackRadius)
            {
                m_owner.m_stateMachine.PopState();
            }
        }

        public override void Exit()
        {
        }
    }

    class SeekState<T> : State<T> where T : Enemy
    {
        public SeekState(T owner) : base(owner) { }

        public override void Enter()
        {
            m_owner.m_speedMultiplier += 1.05f;
        }

        public override void Update()
        {
            m_owner.m_animator.SetBool("Running", true);

            Vector2 direction = (m_owner.m_foe.transform.position - m_owner.transform.position).normalized;
            direction.x /= Mathf.Abs(direction.x);
            m_owner.m_force = Vector2.zero;
            m_owner.m_force = direction * m_owner.m_speed * m_owner.m_speedMultiplier;
            m_owner.m_force.y = 0.0f;

            m_owner.m_currentScale = m_owner.Flip(direction, m_owner.m_startScale, m_owner.m_currentScale);
            m_owner.transform.localScale = m_owner.m_currentScale;
            Vector2 directionDist = m_owner.m_foe.transform.position - m_owner.transform.position;
            float angle = m_owner.GetAngleFromEnemy();
            if (angle > m_owner.m_fov && directionDist.magnitude >= m_owner.m_detectionRadius)
            {
                m_owner.m_stateMachine.PopState();
            }
            else if (angle < m_owner.m_fov * 2.0f && directionDist.magnitude <= m_owner.m_attackRadius)
            {
                m_owner.m_stateMachine.PopState();
            }
        }

        public override void Exit()
        {
            m_owner.m_animator.SetBool("Running", false);
            m_owner.m_speedMultiplier -= 1.05f;
        }
    }

    class JumpState<T> : State<T> where T : Enemy
    {
        public JumpState(T owner) : base(owner) { }

        public override void Enter()
        {
        }

        public override void Update()
        {
            if (m_owner.m_onGround && Mathf.Approximately(m_owner.m_rigidbody2D.velocity.y, 0.0f))
            {
                m_owner.m_rigidbody2D.AddForce(Vector3.up * m_owner.m_jumpForce, ForceMode2D.Impulse);

                m_owner.m_stateMachine.PopState();
            }
        }

        public override void Exit()
        {
        }
    }

    class DropState<T> : State<T> where T : Enemy
    {
        public DropState(T owner) : base(owner) { }

        public override void Enter()
        {

        }

        public override void Update()
        {
            if (m_owner.m_onGround && Mathf.Approximately(m_owner.m_rigidbody2D.velocity.y, 0.0f))
            {
                Platform.Instance.DropEntity(m_owner.gameObject);

                m_owner.m_stateMachine.PopState();
            }
        }

        public override void Exit()
        {

        }
    }
}
