using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HideLastPanelButton : MonoBehaviour
{
    private PanelManager _panelManager;

    private void Start()
    {
        _panelManager = PanelManager.Instance;
    }
    public void DoHidePanel()
    {
        _panelManager.HideLastPanel();

    }
}
