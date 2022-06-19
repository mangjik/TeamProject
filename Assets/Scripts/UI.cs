using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    public Slider HP;
    public Slider MP;

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

}
