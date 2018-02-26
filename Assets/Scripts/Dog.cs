using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : Creature, IMovable
{
    [SerializeField] [Range(1.0f, 5000.0f)] float m_speed = 50.0f;
    [SerializeField] [Range(1.0f, 100.0f)] float m_jumpForce = 17.0f;
    [SerializeField] [Range(1.0f, 20.0f)] float m_runDistinction = 5.0f;
    [SerializeField] [Range(1.0f, 10.0f)] float m_minDistance = 3.0f;
    [SerializeField] [Range(2.0f, 20.0f)] float m_maxDistance = 8.0f;
    [SerializeField] [Range(0.0f, 5.0f)] float m_stateCooldown = 0.5f;
    [SerializeField] public GameObject m_owner = null;
    [SerializeField] Transform m_groundTouch;
    [SerializeField] LayerMask m_groundMask;
    [SerializeField] LayerMask m_platformMask;

    StackStateMachine<Dog> m_stateMachine;
    Animator m_animator;
    Rigidbody2D m_rigidBody2D;
    Vector3 m_startScale;
    Vector3 m_currentScale;
    Vector2 m_force = Vector2.zero;
    float m_speedMultiplier = 1.0f;
    float m_stateTime = 0.0f;
    bool m_onGround = true;

    public float SpeedMultiplier { get { return m_speedMultiplier; } set { m_speedMultiplier = value; } }

    void Start()
    {
        m_stateMachine = new StackStateMachine<Dog>();
        m_stateMachine.AddState("Follow", new FollowState<Dog>(this));
        m_stateMachine.AddState("Jump", new JumpState<Dog>(this));
        m_stateMachine.AddState("Drop", new DropState<Dog>(this));
        m_stateMachine.PushState("Follow");

        m_animator = GetComponent<Animator>();
        m_rigidBody2D = GetComponent<Rigidbody2D>();

        m_startScale = transform.localScale;
        m_currentScale = m_startScale;
    }

    private void Update()
    {
        m_onGround = IsOnGround(m_groundTouch.position, m_groundMask, m_platformMask, 0.2f);
        m_animator.SetBool("OnGround", m_onGround);

        if (m_active)
        {
            m_stateMachine.Update();
        }
    }

    void FixedUpdate()
    {
        m_force *= Time.deltaTime;
        m_force = (m_onGround) ? m_force : m_force * 0.5f;
        m_rigidBody2D.AddForce(m_force);

        m_animator.SetFloat("WalkSpeed", Mathf.Abs(m_rigidBody2D.velocity.x * 0.5f));
        m_animator.SetFloat("RunSpeed", Mathf.Abs(m_rigidBody2D.velocity.x) - m_runDistinction);

        if (m_rigidBody2D.velocity.y > 0.0f && !m_onGround)
        {
            m_rigidBody2D.velocity += ((Vector2.up * Physics2D.gravity.y)) * (m_jumpResistance - 1.0f) * Time.deltaTime;
        }
        else if (m_rigidBody2D.velocity.y < 0.0f && !m_onGround)
        {
            m_rigidBody2D.velocity += ((Vector2.up * Physics2D.gravity.y)) * (m_fallMultiplier - 1.0f) * Time.deltaTime;
        }

        m_animator.SetFloat("yVelocity", m_rigidBody2D.velocity.y);
    }

    class FollowState<T> : State<T> where T : Dog
    {
        public FollowState(T owner) : base(owner) { }

        public override void Enter()
        {
        }

        public override void Update()
        {
            Vector2 direction = m_owner.m_owner.transform.position - m_owner.transform.position;
            float distance = direction.magnitude;
            Vector2 walkDirection = direction.normalized;
            walkDirection.y = 0.0f;
            if (distance > m_owner.m_maxDistance)
            {
                m_owner.m_force = walkDirection * m_owner.m_speed * m_owner.m_speedMultiplier;
            }
            else if (distance < m_owner.m_minDistance)
            {
                walkDirection = -walkDirection;
                m_owner.m_force = walkDirection * m_owner.m_speed * m_owner.m_speedMultiplier;
            }

            m_owner.m_currentScale = m_owner.Flip(walkDirection, m_owner.m_startScale, m_owner.m_currentScale);
            m_owner.transform.localScale = m_owner.m_currentScale;

            m_owner.m_stateTime += Time.deltaTime;

            if (m_owner.m_stateTime > m_owner.m_stateCooldown)
            {
                m_owner.m_stateTime = 0.0f;

                float yDifference = m_owner.m_owner.transform.position.y - m_owner.transform.position.y;
                float xDifference = m_owner.m_owner.transform.position.x - m_owner.transform.position.x;
                if (yDifference > 4.0f && Mathf.Abs(xDifference) < 3.0f)
                {
                    m_owner.m_stateMachine.PushState("Jump");
                }
                else if (yDifference < -3.5f && Mathf.Abs(xDifference) < 2.0f)
                {
                    m_owner.m_stateMachine.PushState("Drop");
                }
            }
        }

        public override void Exit()
        {
        }
    }

    class JumpState<T> : State<T> where T : Dog
    {
        public JumpState(T owner) : base(owner) { }

        public override void Enter()
        {

        }

        public override void Update()
        {
            if (m_owner.m_onGround && Mathf.Approximately(m_owner.m_rigidBody2D.velocity.y, 0.0f))
            {
                m_owner.m_rigidBody2D.AddForce(Vector3.up * m_owner.m_jumpForce, ForceMode2D.Impulse);

                m_owner.m_stateMachine.PopState();
            }
        }

        public override void Exit()
        {

        }
    }

    class DropState<T> : State<T> where T : Dog
    {
        public DropState(T owner) : base(owner) { }

        public override void Enter()
        {
        }

        public override void Update()
        {
            if (m_owner.m_onGround && Mathf.Approximately(m_owner.m_rigidBody2D.velocity.y, 0.0f))
            {
                Platform.Instance.DropEntity(m_owner.gameObject);
            }

            m_owner.m_stateMachine.PopState();
        }

        public override void Exit()
        {

        }
    }
}
