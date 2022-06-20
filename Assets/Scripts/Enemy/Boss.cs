using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    private enum State { None, Idle, Patrol, Trace, Attack, Die }

    [SerializeField] SkillInfo[] m_skillInfos;
    [SerializeField] EnemyBasicInfo m_info;

    [SerializeField]
    private State m_state;

    [SerializeField]
    private float m_time;
    [SerializeField]
    private int m_dir = 0;

    [SerializeField]
    private Transform m_ThrowStartTrans;
    [SerializeField]
    private Transform m_meleeAttackTrans;
    [SerializeField]
    private Vector2 m_meleeAttackBoxSize;

    private bool m_turn = false;
    private bool m_tracing = false;
    [SerializeField]
    private bool m_attacking = false;

    private Rigidbody2D m_rigid;
    private Animator m_animator;

    public float d;

    private void Awake()
    {
        m_state = State.Idle;

        m_rigid = this.GetComponent<Rigidbody2D>();
        m_animator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        Sight();
    }

    private void FixedUpdate()
    {
        if (m_attacking == true) return;
        switch (m_state)
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
        if (m_dir == 1)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (m_dir == -1)
        {
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        Collider2D[] targetColliders =
            Physics2D.OverlapCircleAll(this.transform.position, m_info.ViewDistance);

        foreach (var target in targetColliders)
        {
            if (target.tag != "Player") continue;

            int layerMask = (1 << LayerMask.NameToLayer("Enemy")) + (1 << LayerMask.NameToLayer("Skill"));
            RaycastHit2D hit =
                Physics2D.Raycast(transform.position, target.transform.position - transform.position, m_info.ViewDistance, ~layerMask);
            if (hit == false) continue;

            Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.red);
            if (target.gameObject == hit.transform.gameObject)
            {
                
                Vector2 dir = (target.transform.position - this.transform.position).normalized;

                if (dir.x < 0) m_dir = -1;
                else if (dir.x > 0) m_dir = 1;
                
                if (m_attacking != true && Vector2.Distance(hit.transform.position, this.transform.position) < m_info.AttackDistance)
                {
                    m_state = State.Attack;
                    m_attacking = true;

                    int rand = Random.Range(1, 3);

                    UseSkill(rand);
                    //UseSkill(2);

                    m_rigid.velocity = new Vector2(0, m_rigid.velocity.y);

                    return;
                }
                else if(m_state == State.Idle)
                {
                    m_state = State.Trace;
                    m_tracing = true;
                }
                return;
            }
        }

        if (m_tracing == true)
            SetStateIdle();
    }

    public void UseSkill(int count)
    {
        GameObject Throw = null;
        string triggerName = null;

        SkillInfo skillInfo = m_skillInfos[count];

        Throw = skillInfo.UseSkill(out triggerName);

        m_animator.SetTrigger(triggerName);

        if(Throw == null)
        {
            Collider2D[] colliders =
                Physics2D.OverlapBoxAll(m_meleeAttackTrans.position, skillInfo.BoxSize, 0);

            foreach(var collider in colliders)
            {
                if(collider.tag == "Player")
                {
                    Player player = collider.GetComponent<Player>();
                    player.TakeDamage(m_info.AttackPoint);
                }
            }
        }
        else
        {
            GameObject go = Instantiate(Throw);
            go.transform.position = new Vector3
                (m_ThrowStartTrans.position.x, m_ThrowStartTrans.position.y + go.transform.position.y, 0);
            go.transform.localScale = new Vector2(1, 1);

            SkillObjectControl control = go.GetComponent<SkillObjectControl>();
            if (this.transform.rotation.y == 0)
                control.SetDir(true);
            else
                control.SetDir(false);

            control.SetDelay(skillInfo.Delay);
            control.SetSpeed(skillInfo.Speed);
            control.SetDamage(skillInfo.AttackPoint);
            control.SetRemoving(skillInfo.Removing);
        }
    }

    public void PatrolMove()
    {
        CheckForward();

        if (m_state != State.Patrol) return;
        m_animator.SetBool("Move", true);

        m_rigid.velocity = new Vector2(m_dir * m_info.Speed, m_rigid.velocity.y);

    }

    public void TraceMove()
    {

        CheckForward(1.5f, 2.0f);
        //transform.Translate((Vector2.right * m_dir) * m_info.RunSpeed);
        if (m_state != State.Trace)
        {
            m_rigid.velocity = new Vector2(0, m_rigid.velocity.y);
            m_tracing = false;
            return;
        }
        m_animator.SetBool("Move", true);
        m_rigid.velocity = new Vector2(m_dir * m_info.RunSpeed, m_rigid.velocity.y);
    }

    public void SetStateIdle(float min = 0.5f, float max = 1.0f)
    {
        m_tracing = false;
        m_animator.SetBool("Move", false);

        m_state = State.Idle;
        m_time = Random.Range(min, max);
    }

    private void CheckForward(float min = 0.5f, float max = 1.0f)
    {
        Vector2 frontVec = new Vector2(transform.position.x + m_dir * 2.1f, transform.position.y);

        int layerMask = (1 << LayerMask.NameToLayer("Enemy")) + 
            (1 << LayerMask.NameToLayer("Skill")) + (1<<LayerMask.NameToLayer("Player"));

        RaycastHit2D downHit = Physics2D.Raycast(frontVec, Vector2.down, d, ~layerMask);
        RaycastHit2D frontHit = Physics2D.Raycast(frontVec, Vector2.right * m_dir, 0.5f, ~layerMask);

        if (downHit.collider == null || (frontHit.collider != null && frontHit.transform.tag == "Wall"))
        {
            m_turn = true;
            SetStateIdle(min, max);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Skill")
        {
            SkillObjectControl skill = collision.GetComponent<SkillObjectControl>();
            TakeDamage(skill);
        }
    }

    private void CheckSurvival()
    {
        if (m_info.HP <= 0)
        {
            m_info.Survival = false;
            UI.instance.DecreaseBossCount();
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

    public void EndAttack()
    {
        Debug.Log("IN");
        m_attacking = false;
        SetStateIdle();
    }
}
