using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMSupport;

public class UIItemButton : MonoBehaviour 
{
	public Button						m_Button;
	public Image						m_Image;
	public Scene.eActions				m_Action;
	public EquipmentManager.eEquipment	m_Equipment;
    public BaseActionScriptableObject   ActionSO;
	public Text							m_Text;
	public string						m_String = "TEST";

	RectTransform						m_RectTransform;
	bool								m_Animate;

	// Use this for initialization
	void Start () 
	{	
		m_Animate = false;
		m_RectTransform = gameObject.GetComponent<RectTransform>();
	}

    // Refactor NEW
	public void SetButtonData(BaseActionScriptableObject actionSO, string overrideUIName = null)
    {
        ActionSO = actionSO;

        m_Image.sprite = actionSO.Image;
        m_String = actionSO.Name;
		if (overrideUIName == null) m_Text.text = m_String;
		else {
			m_Text.text = overrideUIName;
		}
    }

    //public void SetButtonData(Sprite NewImage, Scene.eActions Action) 
    //{
    //    m_Image.sprite = NewImage;
    //    m_Action = Action;
    //    m_Equipment = EquipmentManager.eEquipment.MAX;
    //    m_String = m_Action.ToString();
    //    m_Text.text = m_String;
    //}
    //public void SetButtonData(Sprite NewImage, EquipmentManager.eEquipment Equipment)
    //{
    //    m_Image.sprite = NewImage;
    //    m_Action = Scene.eActions.MAX;
    //    m_Equipment = Equipment;
    //    m_String = Equipment.ToString();
    //    m_Text.text = m_String;
    //}

	public void OnClicked()
	{

        if (ActionSO.IsItem() && !ActionSO.IsAction())
        {
            TMMessenger.Send("SceneEquipmentClicked".GetHashCode(), ActionSO);
        }
        else
        {
            TMMessenger.Send("SceneActionClicked".GetHashCode(), ActionSO);
        }

        // REFACTOR: old code:

    //    if(m_Action != Scene.eActions.MAX)
    //        TMMessenger.Send("SceneActionClicked".GetHashCode(), (int)m_Action);
    //    else
    //        TMMessenger.Send("SceneEquipmentClicked".GetHashCode(), (int)m_Equipment);
    }

	internal void Highlight(bool State = true)
	{
		m_Animate = State;
		if(m_Animate == false)
			m_RectTransform.localScale = Vector3.one;
	}

	void Update()
	{
		Vector3			Scale = Vector3.one;

		if(m_Animate)
		{
			Scale.x = Scale.y = 1.0f + Mathf.Sin(Time.time * 3) * 0.16f;
			m_RectTransform.localScale = Scale;
		}
	}
}
