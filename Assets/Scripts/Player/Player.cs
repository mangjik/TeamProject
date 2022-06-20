using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerInfo[] PlayerInfos;
    [SerializeField] int infoCount;


    private PlayerInfo m_playerInfo;
    [SerializeField]
    private PlayerBasicInfo m_info;
    private Skill m_skill;
    private Rigidbody2D m_rigid;
    private Animator m_animator;
    private SpriteRenderer m_spriteRenderer;

    [SerializeField]
    private Transform m_ThrowStartTrans;

    private float m_speed;
    private float m_hor;

    private float m_MP;

    [SerializeField]
    private Transform m_meleeAttackTrans;
    [SerializeField]
    private Vector2 m_meleeAttackBoxSize;

    private AudioSource m_audioSource;

    private void Start()
    {
        m_rigid = this.GetComponent<Rigidbody2D>();
        m_animator = this.GetComponent<Animator>();
        m_spriteRenderer = this.GetComponent<SpriteRenderer>();
        m_audioSource = this.GetComponent<AudioSource>();
        SetPlayerInfo(PlayerInfos[DataSave.instance.infoCount]);

        this.m_animator.runtimeAnimatorController = m_info.animator;

        m_MP = 0;
    }


    private void Update()
    {
        RecoverMP();
        if (Input.GetButtonUp("Horizontal"))
        {
            m_rigid.velocity = new Vector2(m_rigid.velocity.normalized.x * 0.1f, m_rigid.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (m_info.CanJump() == true)
                this.Jump();
        }
        if (Input.GetKeyDown(KeyCode.Z))
            UseSkill(1);
        if (Input.GetKeyDown(KeyCode.X))
            UseSkill(2);
    }

    private void FixedUpdate()
    {
        m_hor = Input.GetAxisRaw("Horizontal");

        this.Move();
    }

    private void RecoverMP()
    {
        if (m_info.MP == m_info.MaxMP) return;
        
        m_MP += Time.deltaTime;

        if(m_MP >= 1.0f)
        {
            m_MP -= 1.0f;
            m_info.MP += 1;

            UI.instance.SetMP(m_info.MP);
        }
    }

    private void SetPlayerInfo(PlayerInfo playerInfo)
    {
        m_playerInfo = new PlayerInfo();
        m_playerInfo = playerInfo;

        m_info = new PlayerBasicInfo(playerInfo.playerBasicInfo);

        m_speed = m_info.Speed;

        UI.instance.Init(m_info.HP, m_info.MaxMP);
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            this.transform.rotation = Quaternion.Euler(0,0,0);
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            this.transform.rotation = Quaternion.Euler(0, 180f,0);

        m_rigid.velocity = new Vector2(m_hor * m_speed, m_rigid.velocity.y);

        m_animator.SetFloat("Run", m_rigid.velocity.x);
    }

    private void Jump()
    {
        m_rigid.velocity = Vector2.up * m_info.JumpForce;

        if (m_info.IsGrounded == true)
        {
            m_info.IsGrounded = false;
            m_animator.SetTrigger("Jump");
            PlaySound(0);
        }
        else if (m_info.DoubleJump == false)
        {
            m_info.DoubleJump = true;
            m_animator.SetTrigger("DoubleJump");
            PlaySound(0);
        }
    }

    public void TouchFloor()
    {
        m_info.TouchFloor();
        m_animator.SetTrigger("IsGrounded");
    }
    public void ExitFloor()
    {
        m_info.IsGrounded = false;
    }


    public void UseSkill(int count)
    {
        GameObject Throw = null;
        string animationName;

        SkillInfo skillInfo;

        if (m_playerInfo.skills.GetSkill(out skillInfo, count) == false) return;
        if (m_playerInfo.UseSkill(count, m_info.MP, 
            out m_info.MP, out animationName, out Throw) == false)
            return;

        UI.instance.SetMP(m_info.MP);

        m_animator.SetTrigger(animationName);

        if (Throw == null)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(m_meleeAttackTrans.position, skillInfo.BoxSize, 0);

            foreach(var collider in colliders)
            {
                if(collider.tag == "Enemy")
                {
                    Enemy enemy = collider.GetComponent<Enemy>();
                    enemy.TakeDamage(skillInfo.AttackPoint);
                    
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
        m_audioSource.clip = skillInfo.Audio;
        m_audioSource.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Enemy")
        {
            m_info.HP -= collision.transform.GetComponent<Enemy>().GetDamage();
            CheckSurvival();
        }
        if(collision.transform.tag == "Boss")
        {
            m_info.HP -= collision.transform.GetComponent<Boss>().GetDamage();
            CheckSurvival();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "BossSkill")
        {
            SkillObjectControl skill = collision.GetComponent<SkillObjectControl>();
            if (skill.GetDealy() > 0) return;
            Debug.Log("TakeDamage");
            m_info.HP -= skill.GetDamage();
            CheckSurvival();
            Destroy(collision.gameObject);
        }
    }

    private void CheckSurvival()
    {
        UI.instance.SetHP(m_info.HP);
        if(m_info.HP <= 0)
        {
            m_info.Survival = false;
            SceneManager.LoadScene("Dead");
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("TakeDamage2");

        m_info.HP -= damage;


        CheckSurvival();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(m_meleeAttackTrans.position, m_meleeAttackBoxSize);
    }


    public void PlaySound(int number)
    {
        m_audioSource.clip = m_info.AudioClips[number];

        m_audioSource.loop = false;
        m_audioSource.Play();
    }
}
