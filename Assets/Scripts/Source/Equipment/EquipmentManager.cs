using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMSupport;

public enum ePPEType
{
	Basic = 0,
	Enhanced
}

public class EquipmentManager : MonoBehaviour
{
	static EquipmentManager				sInstance;
	public static EquipmentManager		Instance { get { return sInstance; } }

    // Refactor: TODO: This is still here as there's a couple of places that still use it and need to be switched over to the BASO setup. Needs removing altogether asap.
	public static bool[]				sChosenEquipment;

	/// <summary>
	/// The choosen equipment
	/// </summary>


	ePPEType		m_ProtectionRequired;
	public ePPEType ProtectionRequired { get { return m_ProtectionRequired; } }

	public enum eEquipment
	{
		// Head
		FaceShield = 0,
		Goggles,
		FaceMask,
		HeadCover,
		Hood,
		AttachedHood,
		// Body
		Scrubs,
		Suit,
		Gown,
		ReusableApron,
		DisposableApron,
		// Hands and feet
		InnerGloves,
		OuterGloves,
		Boots,					// This is used for the boots doffing inspection stage
		ClosedToeShoes,
		// Reserved
		Preparation,
		Gloves,
		Name,
		BootsRemoval,			// This is used for the last boots doffing stage
		MAX,
	}

    private List<GameObject> m_ItemGameObjects;

	[Header("Extra sprites")]
	public Sprite					m_Preparation;

	[Header("Collision layer")]
	public LayerMask				m_LayerMask;
	public LayerMask				m_HighlightLayerMask;
	
	int								m_NumEquipmentChoices;
	
	UIItemSelection					m_UISelection;	
		
	bool							m_WaitForLastBuddySpeechToFinish;
	bool							m_Enabled;
	bool							m_validEquipmentSelectionMade;
	float							m_watchDogTimer;

    public ActionListScriptableObject ActionListBasic, ActionListEnhanced;

    private ActionListScriptableObject m_ActionList;

    public EquippedItems m_EquippedItems;

    public ActionListScriptableObject GetActionList()
    {
        return m_ActionList;
    }

    public void SetActionList(ActionListScriptableObject actionList)
    {
        m_ActionList = actionList;
    }

	void OnDestroy()
	{
		TMMessenger.Instance.RemoveFromMessageQueue(this);
	}

	void Awake()
	{
        ResetEquippedItems();

		sInstance = this;
		TMMessenger.Instance.AddToMessageQueue(new MessageCallback(this.MessageCallback));
		m_Enabled = false;
		m_WaitForLastBuddySpeechToFinish = false;
		m_validEquipmentSelectionMade = false;
		
		m_NumEquipmentChoices = (int)eEquipment.MAX;
					
		sChosenEquipment = new bool[m_NumEquipmentChoices];
		m_ProtectionRequired = ePPEType.Basic;
	}

    private void ResetEquippedItems()
    {
        m_EquippedItems = new EquippedItems();
    }

    // Find all equipment items in the scene and get references to them
	private void SetupEquipmentItems(ActionListScriptableObject actionListSO)
    {
        m_ItemGameObjects = new List<GameObject>();

        GameObject [] items = GameObject.FindGameObjectsWithTag("EquipmentItem");

        foreach (GameObject item in items)
        {            
            if (actionListSO.ShowUnusedEquipment)
            {                
                m_ItemGameObjects.Add(item);
            }
            else
            {
                // Check if the item is part of  the action list and only add it if it is, otherwise hide it
                 Equipment equipment = item.GetComponent<Equipment>();

                if (equipment!=null)
                {
                    if (actionListSO.GetActionOrder().Contains(equipment.ItemSO))
                        m_ItemGameObjects.Add(item);
                    else
                        item.SetActive(false);
                }
            }                      
        }
    }

    // Find our reference to the gameobject for th specified actionSO
    public GameObject GetItemGameObject(BaseActionScriptableObject actionSO)
    {
        foreach (GameObject go in m_ItemGameObjects)
        {
            Equipment equipment = go.GetComponent<Equipment>();

            if (equipment != null)
            {
                if (equipment.ItemSO == actionSO)
                    return go;
            }
        }        
        return null;
    }

    public bool BasicPPE()
    {
        return (m_ProtectionRequired == ePPEType.Basic);
    }


	void MessageCallback(TMMessageNode Message)
	{
		if (Message.Message == "SceneActionClicked".GetHashCode()) {

			if (!m_EquippedItems.IsFullyEquipped(GetActionList())) // Don't allow any changes once we've equipped a full set of items
			RemoveEquipment(Message.ActionSO);
		}
		else if (m_Enabled && m_WaitForLastBuddySpeechToFinish && Message.Message == "BuddyDialogFinished".GetHashCode())
		{
			SendEquipmentChosenMessage();
		}
	}



	public void ChoosenPPE(ePPEType ppeType) {

		if (ppeType == ePPEType.Basic) {

			m_ActionList = ActionListBasic;
			SetupEquipmentItems(GetActionList());
			m_ProtectionRequired = ePPEType.Basic;
			m_Enabled = true;

		}
		else if (ppeType == ePPEType.Enhanced) {

			m_ActionList = ActionListEnhanced; ;
			SetupEquipmentItems(GetActionList());
			m_ProtectionRequired = ePPEType.Enhanced;
			m_Enabled = true;

		}
	}
	  

	public void ChoosenDoffinPPE(ePPEType ppeType) {
		
		if (ppeType == ePPEType.Basic) {
			m_ProtectionRequired = ePPEType.Basic;
		}
		else if (ppeType == ePPEType.Enhanced) {
			m_ProtectionRequired = ePPEType.Enhanced;
		}
	}



	void SendEquipmentChosenMessage()
	{
		TMMessenger.Send("EquipmentChosen".GetHashCode());
		TMMessenger.Instance.RemoveFromMessageQueue(this.MessageCallback);
		m_Enabled = false;
		m_validEquipmentSelectionMade = false;
	}



    private void RemoveEquipment(BaseActionScriptableObject actionSO)
    {
        if (actionSO.HiddenInItemList)
            return;

        m_EquippedItems.RemoveItem(actionSO);
        m_UISelection.RemoveEquipment(actionSO);        
        SetItemVisible(actionSO, true);        
    }



    // When an item on the rack is clicked, this function get's called with the object's associated scriptable object    
    public void ItemClicked(BaseActionScriptableObject actionSO)
    {          
//		Debug.Log("Item clicked " + actionSO.Item.ToString());

        ResetItemUI();

        ActionListScriptableObject actionList = GetActionList();

        // Don't allow any changes once we've equipped a full set of items
        if (m_EquippedItems.IsFullyEquipped(actionList))
            return;


		if (!m_EquippedItems.IsItemEquipped(actionSO)) {

//			Debug.Log("Can equip: " + m_EquippedItems.CanEquipItem(actionSO, actionList));

			//equiping
	        if (m_EquippedItems.CanEquipItem(actionSO, actionList))
	        {
	            m_EquippedItems.EquipItem(actionSO, actionList);

				//hide item when equiping
	            SetItemVisible(actionSO, false);

	            AddEquipmentToChosen(actionSO); 
	                       
	            GameManager.Instance.Buddy.TriggerConversation("Equipment_" + actionSO.Item.ToString(), false);

	            if (m_EquippedItems.IsFullyEquipped(actionList))     
	                EquipmentDonned();                      
	        }
		}
		else {
			//putting back
			//alredy equipped
			RemoveEquipment(actionSO);

		}   
    }


	public bool CanChooseEquipment(BaseActionScriptableObject item) {
	
		ActionListScriptableObject actionList = GetActionList();
		return m_EquippedItems.CanEquipItem(item, actionList);

	}



    private void SetItemVisible(BaseActionScriptableObject actionSO, bool bFlag)
    {
        GameObject itemGO = GetItemGameObject(actionSO);

        if (itemGO != null)
            itemGO.SetActive(bFlag);
    }

    private void EquipmentDonned()
    {
        SetupFromChosenEquipment(m_EquippedItems);        

        m_WaitForLastBuddySpeechToFinish = true;        
        m_validEquipmentSelectionMade = true;
        m_watchDogTimer = 0;
    }
  
    private void ResetItemUI()
    {
        if (m_UISelection == null)
        {
            m_UISelection = GameObject.FindObjectOfType<UIItemSelection>();
            if (m_UISelection != null)
                m_UISelection.ResetItems();
        }
    }



	/// <summary>
	/// Called on end of choosing equipment
	/// </summary>
	/// <param name="equippedItems">Equipped items.</param>
    private void SetupFromChosenEquipment(EquippedItems equippedItems)
    {
        string ClothingOption = "";

        ResetItemUI();
            

        // Loop through all the equipped items
        foreach (BaseActionScriptableObject so in equippedItems.GetOrderedItems(GetActionList()))
        {
            if (so.IsItem())
                ClothingOption += so.Item.ToString() + ",";
        }
        
		//saving
        UserProfile.sCurrent.SetEquipmentDescription(ClothingOption);

        Scene1 DonningScene = GameObject.FindObjectOfType<Scene1>();

        DonningScene.SetupStatesPostEquipment();
    }
	  	


	// refactor v2 remove when can
    public void AddPreparationStep()
    {
        Debug.Log("REFACTOR: Remove this call when possible! EquipmentManager::AddPreparationStep()");

        foreach (BaseActionScriptableObject action in GetActionList().GetActionOrder())
        {
            if (action.IsAction())
            {
                if (action.Action == BaseActionScriptableObject.ACTION_TYPE.PREPARATION)
                {
                    m_UISelection.AddEquipment(action);
                    return;
                }
            }
        }                
    }




    void AddEquipmentToChosen(BaseActionScriptableObject actionSO)
    {       

		if (m_UISelection != null && !actionSO.HiddenInItemList) m_UISelection.AddEquipment(actionSO);

        sChosenEquipment[(int)actionSO.Item] = true;        
    }




	void Update()
	{
		if( m_WaitForLastBuddySpeechToFinish == true && m_validEquipmentSelectionMade == true )
		{
			m_watchDogTimer+=Time.deltaTime;
			if( m_watchDogTimer > 5 )
			{
				Debug.LogError( "Watchdog timer forced advance to next state." );
				SendEquipmentChosenMessage();
				m_WaitForLastBuddySpeechToFinish = false;
				m_Enabled = false;
			}
		}

		if(m_Enabled == false)
			return;
   
	}




	public bool CanChooseEquipment(Equipment equipment) {
		return (m_ItemGameObjects.Contains(equipment.gameObject));
	}






    // REFACTOR: Temp replacement function to fill our equipped items from the save string.
    internal void CreateChosenStatesFromSave(ActionListScriptableObject actionList)
    {
        ResetEquippedItems();
        ResetItemUI();

        string EquipmentOptions = UserProfile.sCurrent.EquipmentDescriptionString;

        if (EquipmentOptions.Length > 0)
        {
            Debug.Log("Load string: " + EquipmentOptions);

            string[] EquipmentArray = EquipmentOptions.Split(',');

            for (int i = 0; i < EquipmentArray.Length; i++)
            {
                foreach (BaseActionScriptableObject actionSO in actionList.GetActionOrder())
                {
                    if (actionSO.Item.ToString() == EquipmentArray[i])
                    {
//                        Debug.Log("Item found! " + actionSO.Item.ToString());
                        m_EquippedItems.EquipItem(actionSO, actionList);
                        AddEquipmentToChosen(actionSO);
                    }
                }               
            }           
        }             
    }



	internal void SetDoffingEquipment(ActionListScriptableObject actionList) {

		ResetEquippedItems();
		ResetItemUI();

		foreach (BaseActionScriptableObject actionSO in actionList.GetActionOrder())
		{
			if (actionSO.IsItem()) {
				m_EquippedItems.EquipItem(actionSO, actionList);
				AddEquipmentToChosen(actionSO);
			}
		
		}          


	}






	internal void DressAvatar(Avatar TheAvatar)
	{
		TheAvatar.SetBaseClothing();
		for(int loop = 0; loop < sChosenEquipment.Length; loop++)
		{
			if (sChosenEquipment[loop] == true)
				TheAvatar.PutOn((eEquipment)loop);
		}
	}

    public EquippedItems GetEquippedItems()
    {
        return m_EquippedItems;
    }
}





