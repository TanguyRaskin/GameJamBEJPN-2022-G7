using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager s_CanvasInstance;

    public GameObject m_PauseMenu;
    public GameObject m_GameOverMenu;
    public GameObject m_EndPanel;

    private void Awake()
    {
        if (s_CanvasInstance == null)
            s_CanvasInstance = this;
        else
            Destroy(gameObject);
    }
    public void SetPause (bool enabled)
    {
        m_PauseMenu.SetActive(enabled);
    }
    public void SetGameOver(bool enabled)
    {
        m_GameOverMenu.SetActive(enabled);
    }
    public void SetEnd(bool enabled)
    {
        m_EndPanel.SetActive(enabled);
    }
}
