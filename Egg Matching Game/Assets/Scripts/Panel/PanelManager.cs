using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

// PanelID enum is defined elsewhere
public class PanelManager : Singleton<PanelManager>
{
    // List that stores panel instances
    public List<PanelInstanceModel> _listInstance = new List<PanelInstanceModel>();

    // Reference to the object pool
    private ObjectPool _objectPool;

    private void Start()
    {
        // Get instance of the object pool
        _objectPool = ObjectPool.Instance;
        if (_objectPool == null)
        {
            Debug.LogError("ObjectPool instance is null. Make sure ObjectPool is initialized before PanelManager.");
        }
    }

    // Function to show a panel (updated to use enum)
    public void ShowPanel(PanelID panelID, PanelShowBehavior behavior = PanelShowBehavior.SHOW_PREVISE)
    {
      

      bool isActive = false;
        if (_objectPool == null)
        {
            _objectPool = ObjectPool.Instance;
        }
        foreach (Transform childTransform in _objectPool.transform)
        {
            if (childTransform.name == panelID.ToString())
            {
                isActive = childTransform.gameObject.activeInHierarchy;
                break;
            }
        }

        if (isActive)
        {
            Debug.Log("panel allredy active "+ panelID.ToString());
        }else
        {
            // Convert enum to string to get panel
            string panelIDString = panelID.ToString();

            // Get panel instance from the object pool
            GameObject instancePanel = _objectPool.GetObjectFromPool(panelIDString);

            // If the panel object is found
            if (instancePanel != null)
            {
                // If the behavior is to hide the previous panel and there's at least one active panel
                if (behavior == PanelShowBehavior.HIDE_PREVISE && GetAmountPanelInList() > 0)
                {
                    var lastPanel = GetLastPanel();
                    if (lastPanel != null)
                    {
                        lastPanel.PanelInstance.SetActive(false);
                    }
                }

                // Add the new panel to the list
                _listInstance.Add(new PanelInstanceModel
                {
                    PanelID = panelID, // Enum stored as string
                    PanelInstance = instancePanel
                });
            }
            else
            {
                Debug.LogWarning($"Panel not found: {panelID}");
            }
        }
    }

    // Function to hide the last panel
    public void HideLastPanel()
    {
        if (AnyPanelIsShowing())
        {
            var lastPanel = GetLastPanel();

            _listInstance.Remove(lastPanel);
            _objectPool.PoolObject(lastPanel.PanelInstance);

            // If there's still a panel left in the list, show it
            if (GetAmountPanelInList() > 0)
            {
                lastPanel = GetLastPanel();

                if (lastPanel != null && !lastPanel.PanelInstance.activeInHierarchy)
                {
                    lastPanel.PanelInstance.SetActive(true);
                }
            }
        }
        // else
        // Debug.LogWarning("No panel is currently open");
    }

    // Function to hide all panels
    public void HideAllPanel()
    {
        // Keep hiding panels until none are left
        while (AnyPanelIsShowing())
        {
            var lastPanel = GetLastPanel();
            _listInstance.Remove(lastPanel);
            _objectPool.PoolObject(lastPanel.PanelInstance);
        }
    }

    // Returns the last panel in the list
    PanelInstanceModel GetLastPanel()
    {
        if (_listInstance.Count == 0)
        {
            return null;
        }
        return _listInstance[_listInstance.Count - 1];
    }

    // Checks if any panel is currently showing
    public bool AnyPanelIsShowing()
    {
        return GetAmountPanelInList() > 0;
    }

    // Returns the number of panels currently showing
    public int GetAmountPanelInList()
    {
        return _listInstance.Count;
    }
}
