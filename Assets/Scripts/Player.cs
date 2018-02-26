using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player : Creature, IMovable
{
    [SerializeField] [Range(1.0f, 100.0f)] float m_health = 100.0f;
    [SerializeField] [Range(1.0f, 5000.0f)] float m_speed = 50.0f;
    [SerializeField] [Range(1.0f, 500.0f)] float m_jumpForce = 20.0f;
    [SerializeField] [Range(1.0f, 100.0f)] float m_damage = 20.0f;
    [SerializeField] [Range(0.0f, 3.0f)] float m_fireRate = 0.5f;
    [SerializeField] [Range(0.0f, 3.0f)] float m_attackRate = 0.5f;
    [SerializeField] public List<Image> m_bulletSprites = new List<Image>();
    [SerializeField] Weapon m_weapon = null;
    [SerializeField] public Transform m_levelStartPoint = null;
    [SerializeField] GameObject m_weaponHitbox = null;
    [SerializeField] ParticleSystem m_slideSmoke = null;
    [SerializeField] Transform m_groundTouch = null;
    [SerializeField] LayerMask m_groundMask = 0;
    [SerializeField] LayerMask m_platformMask = 0;

    Animator m_animator;
    SpriteRenderer m_spriteRenderer;
    Rigidbody2D m_rigidBody2D;
    Vector3 m_startScale;
    Vector3 m_currentScale;
    Vector2 m_force = Vector2.zero;
    float m_speedMultiplier = 1.0f;
    float m_fireTime = 0.0f;
    float m_attackTime = 0.0f;
    int m_ammunition = 0;
    bool m_onGround = true;
    bool m_slid = false;
    bool m_playedSlideSound = false;

    static Player ms_player = null;

    public float SpeedMultiplier { get { return m_speedMultiplier; } set { m_speedMultiplier = value; } }
    public float AttackDamage { get { return m_damage; } }
    public bool IsAlive { get { return m_health > 0.0f; } }

    void Start()
    {
        if (ms_player == null)
        {
            ms_player = this;
        }
        else if (ms_player != this)
        {
            Destroy(ms_player.gameObject);
            ms_player = this;
        }

        DontDestroyOnLoad(gameObject);

        m_animator = GetComponent<Animator>();
        m_rigidBody2D = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        m_startScale = transform.localScale;
        m_currentScale = m_startScale;

        transform.position = m_levelStartPoint.position;
    }
    
    void Update()
    {
        m_onGround = IsOnGround(m_groundTouch.position, m_groundMask, m_platformMask, 0.2f);
        m_animator.SetBool("OnGround", m_onGround);

        if (IsAlive && m_active)
        {
            UpdatePlayer();
        }
    }

    private void FixedUpdate()
    {
        if (IsAlive && m_active)
        {
            PhysicsUpdatePlayer();
        }

        if (m_onGround)
        {
            m_animator.SetFloat("Walk", m_rigidBody2D.velocity.magnitude);
            m_animator.SetFloat("WalkSpeed", m_rigidBody2D.velocity.magnitude * 0.15f);
        }
        else
        {
            m_animator.SetFloat("Walk", 0.0f);
            m_animator.SetFloat("WalkSpeed", 0.0f);
        }
    }

    void UpdatePlayer()
    {
        m_fireTime += Time.deltaTime;
        m_attackTime += Time.deltaTime;
        
        if (Input.GetButtonDown("Fire1") && m_fireTime >= m_fireRate && m_ammunition > 0)
        {
            m_fireTime = 0.0f;
            m_animator.SetTrigger("Shoot");
        }
        if (Input.GetButtonDown("Jump") && m_onGround)
        {
            Vector2 velocity = m_rigidBody2D.velocity;
            velocity.y = 0.0f;
            m_rigidBody2D.velocity = velocity;
            m_rigidBody2D.AddForce(Vector3.up * m_jumpForce, ForceMode2D.Impulse);

            AudioManager.Instance.PlayClip("PlayerJump");
        }
        if (Input.GetButtonDown("Melee") && m_attackTime >= m_attackRate)
        {
            m_attackTime = 0.0f;
            m_animator.SetTrigger("Melee");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            m_animator.SetTrigger("Die");
        }
        if (Input.GetButtonDown("Fire3") && m_onGround)
        {
            m_animator.SetTrigger("Slide");
        }

        m_currentScale = Flip(m_force, m_startScale, m_currentScale);
        transform.localScale = m_currentScale;
    }

    void PhysicsUpdatePlayer()
    {
        m_force = Vector2.zero;

        m_force.x = Input.GetAxis("Horizontal") * m_speed * m_speedMultiplier;

        m_force *= Time.deltaTime;
        m_force = (m_onGround) ? m_force : m_force * 0.5f;
        m_rigidBody2D.AddForce(m_force, ForceMode2D.Force);

        ParticleParty();

        if (m_onGround)
        {
            m_animator.SetFloat("Walk", m_rigidBody2D.velocity.magnitude);
            m_animator.SetFloat("WalkSpeed", m_rigidBody2D.velocity.magnitude * 0.15f);
        }
        else
        {
            m_animator.SetFloat("Walk", 0.0f);
            m_animator.SetFloat("WalkSpeed", 0.0f);
        }

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

    void ParticleParty()
    {
        //print(m_force.x < 0.0f && m_rigidBody2D.velocity.x > 0.0f);
        if (Mathf.Approximately(m_rigidBody2D.velocity.y, 0.0f))
        {
            if (m_force.x < 0.0f && m_rigidBody2D.velocity.x > 0.0f)
            {
                m_slideSmoke.Play();
                m_slid = true;
            }
            else if (m_force.x > 0.0f && m_rigidBody2D.velocity.x < 0.0f)
            {
                m_slideSmoke.Play();
                m_slid = true;
            }
            else
            {
                m_slid = false;
                m_playedSlideSound = false;
            }
        }
        else
        {
            m_slideSmoke.Stop();
        }

        if (m_slid && !m_playedSlideSound)
        {
            AudioManager.Instance.PlayClip("Slide");

            m_playedSlideSound = true;
            m_slid = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponentInParent<Enemy>();
        if (enemy && collision.CompareTag("EvilWeapon"))
        {
            KnockBack((transform.position - enemy.transform.position).normalized * 4.0f);
            LoseHealth(enemy.AttackDamage);
            enemy.HideWeaponHitbox();
        }
    }

    private void LoseHealth(float damage)
    {
        if (IsAlive)
        {
            m_health -= damage;

            if (!IsAlive)
            {
                Die();
            }
            else
            {
                AudioManager.Instance.PlayClip("PlayerGrunt");
            }
        }
    }

    private void Die()
    {
        m_animator.SetTrigger("Die");
        m_animator.SetBool("IsAlive", IsAlive);

        GetComponent<Collider2D>().enabled = false;
        m_rigidBody2D.gravityScale = 0.0f;
        m_rigidBody2D.velocity = Vector2.zero;

        AudioManager.Instance.PlayClip("PlayerDie");

        StartCoroutine(FadeCorpse());
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

        Respawn();
    }

    public void Respawn()
    {
        if (m_levelStartPoint)
        {
            transform.position = m_levelStartPoint.position;
            bool hasHealthUpgrade = UpgradeManager.Instance.HasUpgrade(UpgradeManager.PotentialUpgrades.HEALTH);
            m_health = (hasHealthUpgrade) ? 100.0f * 2.0f : 100.0f;

            if (m_animator)
            {
                m_animator.SetBool("IsAlive", IsAlive);
            }

            GetComponent<Collider2D>().enabled = true;

            if (m_rigidBody2D)
            {
                m_rigidBody2D.gravityScale = 1.0f;
            }

            if (m_spriteRenderer)
            {
                Color c = m_spriteRenderer.color;
                c.a = 1.0f;
                m_spriteRenderer.color = c;
            }

            bool hasGun = UpgradeManager.Instance.HasUpgrade(UpgradeManager.PotentialUpgrades.GUN);
            m_ammunition = (hasGun) ? 4 : 0;

            bool hasDagger = UpgradeManager.Instance.HasUpgrade(UpgradeManager.PotentialUpgrades.DAGGER_RANGE);
            if (hasDagger) m_weaponHitbox.GetComponent<CircleCollider2D>().radius = 2.25f;

            if (m_bulletSprites != null)
            {
                for (int i = m_ammunition - 1; i >= 0; --i)
                {
                    if (m_bulletSprites[i])
                        m_bulletSprites[i].gameObject.SetActive(true);
                }
            }
        }
    }

    public void KnockBack(Vector2 force)
    {
        m_rigidBody2D.AddForce(force, ForceMode2D.Impulse);
    }

    private void PlayFootstep()
    {
        AudioManager.Instance.PlayClip("Footstep");
    }

    private void ShootBullet()
    {
        if (m_ammunition > 0)
        {
            --m_ammunition;
            Vector2 direction = (transform.localScale.x < 0.0f) ? Vector2.left : Vector2.right;
            m_weapon.FireBullet(direction);

            AudioManager.Instance.PlayClip("Gunshot");

            m_bulletSprites[m_ammunition].gameObject.SetActive(false);
        }
    }

    private void PlaySwingSound()
    {
        AudioManager.Instance.PlayClip("PlayerSwing");
    }

    private void ShowWeaponHitbox()
    {
        m_weaponHitbox.SetActive(true);
    }

    public void HideWeaponHitbox()
    {
        m_weaponHitbox.SetActive(false);
    }
}
