using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIItemSelection : MonoBehaviour
{
	public RectTransform						m_ItemPanel;
	public GameObject							m_ButtonPrefab;

	private List<Scene.eActions>				m_ScrollItems;
	private List<BaseActionScriptableObject>	m_Equipment;
	private List<Sprite>						m_ItemImages;

	List<UIItemButton>							m_ItemButtons;

	UIItemButton[]								m_OtherItemButtons;

	Scene										m_SceneManager;
	
	void Awake()
	{
		m_OtherItemButtons = GameObject.FindObjectsOfType<UIItemButton>();
	}

	void Start () 
	{
		m_SceneManager = FindObjectOfType<Scene>();
		if (m_SceneManager == null)
			Debug.Log("SCENE NOT FOUND");
	}



    void CreateButton(BaseActionScriptableObject actionSO)
    {
        UIItemButton NewButton = InstantiateButton();

		if (EquipmentManager.Instance.BasicPPE())
		{
			if (actionSO.IsItem()) {
				if (actionSO.Item == BaseActionScriptableObject.ITEM.InnerGloves) {
					//override name for basic PPE as we are using only one pair of gloves
					NewButton.SetButtonData(actionSO, "Gloves");
				} 
				else NewButton.SetButtonData(actionSO);
			} 
			else NewButton.SetButtonData(actionSO);
		}
		else NewButton.SetButtonData(actionSO);
    }


  
    private UIItemButton InstantiateButton()
    {
        GameObject NewButtonObj = Instantiate(m_ButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        UIItemButton NewButton = NewButtonObj.GetComponent<UIItemButton>();

        m_ItemButtons.Add(NewButton);

        NewButtonObj.transform.SetParent(m_ItemPanel.transform, false);

        return NewButton;
    }
   


    internal void Highlight(BaseActionScriptableObject actionSO, bool bFlag=true)
    {
        int i = 0;
        foreach (BaseActionScriptableObject action in m_Equipment)
        {
            if (action == actionSO)
            {
                m_ItemButtons[i].Highlight(bFlag);
                return;
            }
            i++;
        }

        // Now check any scene buttons that aren't tied to equipment
        for (i = 0; i < m_OtherItemButtons.Length; i++)
        {
            if (m_OtherItemButtons[i].ActionSO == actionSO)
            {
                m_OtherItemButtons[i].Highlight(bFlag);
                return;
            }
        }
    }

    // REFACTOR todo: Remove this later, it's still neded by scene3 at the moment
    internal void Highlight(EquipmentManager.eEquipment equipment, bool State = true)
    {
        Debug.Log("REFACT call to old function: UIItemSelection::Highlight() " + equipment);
    }

    // REFACTOR todo: Remove this later, it's still neded by scene3 at the moment
	internal void Highlight(Scene.eActions Action, bool State = true)
	{
		for (int loop = 0; loop < m_ScrollItems.Count; loop++)
		{
			if(m_ItemButtons[loop].m_Action == Action)
			{
				m_ItemButtons[loop].Highlight(State);
				return;
			}
		}
		for (int loop = 0; loop < m_OtherItemButtons.Length; loop++)
		{            
			if (m_OtherItemButtons[loop].m_Action == Action)
			{
				m_OtherItemButtons[loop].Highlight(State);
				return;
			}
		}
	}
   
	internal void ResetItems()
	{
		m_ScrollItems = null;
		m_ItemImages = null;
		m_ItemButtons = null;
		m_Equipment = null;

		if(m_ScrollItems == null)
			m_ScrollItems = new List<Scene.eActions>(20);
		if (m_ItemImages == null)
			m_ItemImages = new List<Sprite>(20);
		if(m_ItemButtons == null)
			m_ItemButtons = new List<UIItemButton>(20);
		if (m_Equipment == null)
			m_Equipment = new List<BaseActionScriptableObject>(20);
	}



    
    internal void AddEquipment(BaseActionScriptableObject actionSO)
    {
        if (!m_Equipment.Contains(actionSO))
        {
            m_ScrollItems.Add(Scene.eActions.MAX);
            m_Equipment.Add(actionSO);
            m_ItemImages.Add(actionSO.Image);

            CreateButton(actionSO);
        }
    }  




	internal void RemoveEquipment(BaseActionScriptableObject actionSO)
	{
		for(int loop = 0; loop < m_ItemButtons.Count; loop++)
		{
            if (m_ItemButtons[loop].ActionSO == actionSO)
			{
				m_ItemButtons[loop].transform.SetParent(null); 
				GameObject.Destroy(m_ItemButtons[loop].gameObject);

				m_ScrollItems.RemoveAt(loop);
				m_ItemImages.RemoveAt(loop);
				m_ItemButtons.RemoveAt(loop);
				m_Equipment.RemoveAt(loop);
			}
		}
	}
}
