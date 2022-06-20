using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    public Slider HP;
    public Slider MP;
    public int BossCount;
    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void Init(float hp, float mp)
    {
        HP.maxValue = hp;
        MP.maxValue = mp;
        HP.value = hp;
        MP.value = mp;
    }

    public void SetHP(float hp)
    {
        HP.value = hp;
    }
    public void SetMP(float mp)
    {
        MP.value = mp;
    }

    public void DecreaseBossCount()
    {
        BossCount -= 1;
        if(BossCount<1)
        {
            SceneManager.LoadScene("End");
        }
    }

}
