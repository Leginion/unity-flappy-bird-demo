using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject GameOverPanel;

    public void ShowGameOverPanel(bool is_show)
    {
        GameOverPanel.SetActive(is_show);
    }
}
