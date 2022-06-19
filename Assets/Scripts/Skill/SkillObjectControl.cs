using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObjectControl : MonoBehaviour
{
    [SerializeField]
    private float m_speed = 10f;
    private float m_time = 2f;
    private int m_dir = 0;
    private float m_delay;
    private int m_damage;
    private bool m_removing;

    private Animator m_animator;
    private SpriteRenderer m_renderer;


    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_renderer = GetComponent<SpriteRenderer>();

        m_animator.enabled = false;
        m_renderer.enabled = false;
    }

    void Update()
    {
        if(m_delay > 0 )
        {

            m_delay -= Time.deltaTime;
            return;
        }
        if (m_animator.enabled == false || m_renderer.enabled == false)
        {
            m_renderer.enabled = true;
            m_animator.enabled = true;

            if(m_dir == 1)
                m_renderer.flipX = false;
            else if(m_dir == -1)
                m_renderer.flipX = true;
        }

        this.transform.Translate((Vector3.right * m_dir) * (Time.deltaTime * m_speed));

        if (m_time <= 0) Destroy(this.gameObject);
        else m_time -= Time.deltaTime;

    }

    public void SetDir(bool direction)
    {
        if (direction == true)
        {
            m_dir = 1;
        }
        else
        {
            m_dir = -1;
        }
    }

    public void CheckingToRemoveSkill()
    {
        if(m_removing == true)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetDelay(float delay)
    {
        m_delay = delay;
    }
    public float GetDealy()
    {
        return m_delay;
    }
    public void SetSpeed(float speed)
    {
        m_speed = speed;
    }
    public void SetDamage(int damage)
    {
        m_damage = damage;
    }
    public int GetDamage()
    {
        return m_damage;
    }
    public void SetRemoving(bool removing)
    {
        m_removing = removing;
    }
}
