using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public CanvasGroup m_pickCanvas;

    public void ClickPlayBtn()
    {
        m_pickCanvas.alpha = 1.0f;
        m_pickCanvas.blocksRaycasts = true;
    }

    public void ClickExitBtn()
    {
        Application.Quit();
    }

}
