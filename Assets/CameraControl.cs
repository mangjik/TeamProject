using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Transform m_player;

    private void Awake()
    {
        m_player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = 
            new Vector3(m_player.transform.position.x, m_player.transform.position.y, -10f);
    }
}
