using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerBasicInfo : BasicInfo
{
    public int MaxMP;
    public int MP;
    public int JumpForce;

    public bool IsGrounded;
    public bool DoubleJump;

    public RuntimeAnimatorController animator;
    public AudioClip[] AudioClips;
    public PlayerBasicInfo(int hp, int mp, int attackPoint, float speed, int jumpForce)
    {
        this.Survival = true;
        this.HP = hp;
        this.MaxMP = mp;
        this.MP = mp;
        this.AttackPoint = attackPoint;
        this.Speed = speed;
        this.JumpForce = jumpForce;

        this.IsGrounded = false;
        this.DoubleJump = false;
    }

    public PlayerBasicInfo(PlayerBasicInfo playerBasicInfo)
    {
        this.Survival = true;
        this.HP = playerBasicInfo.HP;
        this.MaxMP = playerBasicInfo.MaxMP;
        this.MP = this.MaxMP;
        this.AttackPoint = playerBasicInfo.AttackPoint;
        this.Speed = playerBasicInfo.Speed;
        this.JumpForce = playerBasicInfo.JumpForce;

        this.IsGrounded = false;
        this.DoubleJump = false;

        this.animator = playerBasicInfo.animator;
    }

    public bool CanJump()
    {
        return (IsGrounded == true || DoubleJump == false) ? true : false;
    }

    public void TouchFloor()
    {
        IsGrounded = true;
        DoubleJump = false;

    }

}
