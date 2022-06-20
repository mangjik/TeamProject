using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    public void ClickCountinueBtn()
    {
        SceneManager.LoadScene("Title");
    }

    public void ClickExitBtn()
    {
        Application.Quit();
    }

 }

