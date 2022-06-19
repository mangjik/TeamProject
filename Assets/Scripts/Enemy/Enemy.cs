using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private enum State { None, Idle, Patrol, Trace, Attack, Die }

    [SerializeField] EnemyInfo m_enemyInfo;
    private EnemyBasicInfo m_info;
    [SerializeField]
    private State m_state;

    [SerializeField]
    private float m_time;
    [SerializeField]
    private int m_dir = 0;

    private bool m_turn = false;
    private bool m_tracing = false;

    private Rigidbody2D m_rigid;
    private SpriteRenderer m_renderer;
    private Animator m_animator;

    private void Awake()
    {
        m_state = State.Idle;
        m_info = new EnemyBasicInfo(m_enemyInfo.enemyBasicInfo);

        m_rigid = this.GetComponent<Rigidbody2D>();
        m_renderer = this.GetComponent<SpriteRenderer>();
        m_animator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        Sight();
    }

    private void FixedUpdate()
    {
        switch(m_state)
        {
            case State.Idle:
                if (m_time > 0)
                {
                    m_rigid.velocity = new Vector2(0, m_rigid.velocity.y);
                    m_time -= Time.deltaTime;
                }
                else
                {
                    m_state = State.Patrol;
                    if (m_turn == true)
                    {
                        m_turn = false;
                        m_dir *= -1;
                    }
                    else
                    {
                        while (true)
                        {
                            m_dir = Random.Range(-1, 2);
                            if (m_dir != 0) break;
                        }
                    }
                    m_time = Random.Range(1.0f, 2.0f);
                    PatrolMove();
                }
                break;
            case State.Patrol:

                if (m_time > 0)
                {
                    m_time -= Time.deltaTime;
                    PatrolMove();
                }
                else
                {
                    SetStateIdle();
                }
                break;
            case State.Trace:
                TraceMove();
                break;
            default:
                break;

        }
    }

    public void Sight()
    {
        if(m_dir == 1)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0); 
        }
        else if(m_dir == -1)
        {
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        Collider2D[] targetColliders =
            Physics2D.OverlapCircleAll(this.transform.position,m_info.ViewDistance);

        foreach(var target in targetColliders)
        {
            if (target.tag != "Player") continue;

            int layerMask = (1 << LayerMask.NameToLayer("Enemy")) + (1 << LayerMask.NameToLayer("Skill"));
            RaycastHit2D hit =
                Physics2D.Raycast(transform.position, target.transform.position - transform.position, m_info.ViewDistance, ~layerMask);
            if (hit == null) continue;

            Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.red);
            if (target.gameObject == hit.transform.gameObject)
            {
                m_state = State.Trace;
                m_tracing = true;

                Vector2 dir = (target.transform.position - this.transform.position).normalized;

                if(dir.x < 0) m_dir = -1;
                else if(dir.x > 0) m_dir = 1;

                return;
            }
        }

        if (m_tracing == true)
            SetStateIdle();
    }

    public void PatrolMove()
    {
        /*transform.Translate((Vector2.right * m_dir) * m_info.Speed * Time.deltaTime)*/;
        switch(m_info.Type)
        {
            case EnemyBasicInfo.MonsterType.Chest:
            case EnemyBasicInfo.MonsterType.Earth:
            case EnemyBasicInfo.MonsterType.SlimeSquare:
                m_animator.SetBool("Move", true);
                break;
            default:
                break;
        }

        m_rigid.velocity = new Vector2(m_dir * m_info.Speed, m_rigid.velocity.y);

        CheckForward();
    }

    public void TraceMove()
    {
        CheckForward(1.5f, 2.0f);

        if(m_state != State.Trace)
        {
            m_rigid.velocity = new Vector2(0, m_rigid.velocity.y);
            m_tracing = false;
            return;
        }

        switch (m_info.Type)
        {
            case EnemyBasicInfo.MonsterType.Chest:
            case EnemyBasicInfo.MonsterType.Earth:
            case EnemyBasicInfo.MonsterType.SlimeSquare:
                m_animator.SetBool("Move", true);
                break;
            default:
                break;
        }

        //transform.Translate((Vector2.right * m_dir) * m_info.RunSpeed);
        m_rigid.velocity = new Vector2(m_dir * m_info.RunSpeed, m_rigid.velocity.y);

    }

    public void SetStateIdle(float min = 0.5f, float max = 1.0f)
    {
        switch (m_info.Type)
        {
            case EnemyBasicInfo.MonsterType.Chest:
            case EnemyBasicInfo.MonsterType.Earth:
            case EnemyBasicInfo.MonsterType.SlimeSquare:
                m_animator.SetBool("Move", false);
                break;
            default:
                break;
        }

        m_state = State.Idle;
        m_time = Random.Range(min, max);
    }

    private void CheckForward(float min = 0.5f, float max = 1.0f)
    {
        Vector2 frontVec = new Vector2(transform.position.x + m_dir * 0.8f, transform.position.y);

        int layerMask = (1 << LayerMask.NameToLayer("Enemy")) + (1 << LayerMask.NameToLayer("Skill"))
            + (1<<LayerMask.NameToLayer("Player"));

        RaycastHit2D downHit = Physics2D.Raycast(frontVec, Vector2.down, 1, ~layerMask);
        RaycastHit2D frontHit = Physics2D.Raycast(frontVec, Vector2.right * m_dir, 0.2f, ~layerMask);

        Debug.DrawRay(frontVec, Vector2.down, new Color(0, 1, 0));
        if (downHit.collider == null || (frontHit.collider != null && frontHit.transform.tag == "Wall"))
        {
            m_turn = true;
            SetStateIdle(min, max);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Skill")
        {
            SkillObjectControl skill = collision.GetComponent<SkillObjectControl>();
            TakeDamage(skill);
        }
    }

    private void CheckSurvival()
    {
        if(m_info.HP <= 0)
        {
            m_info.Survival = false;

            Destroy(this.gameObject);
        }

    }

    public int GetDamage()
    {
        return m_info.AttackPoint;
    }

    public void TakeDamage(int damage)
    {
        m_info.HP -= damage;

        CheckSurvival();
    }

    public void TakeDamage(SkillObjectControl skill)
    {
        m_info.HP -= skill.GetDamage();

        skill.CheckingToRemoveSkill();

        CheckSurvival();
    }

}
