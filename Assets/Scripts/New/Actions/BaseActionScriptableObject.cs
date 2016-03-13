using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BaseActionScriptableObject : ScriptableObject {

    public enum ACTION_TYPE
    {
        DON,
        DOFF,
        WASH_HANDS,
        INSPECT,
        PREPARATION,
        NAME_BADGE,
        Count,
		TAKE_TEMPERATURE,
		ASK_QUESTIONS,
        NONE
    };

    public enum ITEM
    {
        // Head
        FaceShield,
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
        Boots,					
        ClosedToeShoes,

        Count,
        NONE,
    }

    public enum EXCLUSIVE_CATEGORY
    {
        Face,
        Head,
        Apron,
        Feet,        
        Body,

        Count,
        NONE,
    }

    public ACTION_TYPE Action = ACTION_TYPE.NONE;        
    public ITEM Item = ITEM.NONE;
    public EXCLUSIVE_CATEGORY Category = EXCLUSIVE_CATEGORY.NONE;
    public Sprite Image;
    public string Name;
    public bool WashHandsAter;
    public bool Automatic;
    public bool Optional;
    public bool HiddenInItemList = false;
	public List<RightWrongQuiz> rightWrongSet = new List<RightWrongQuiz>();
	public string conversationId = "";

	[Header("Basic PPE")]
	public Conversation basicPPEselectConversation;
	public Conversation basicPPEanimationConversation;
	[Header("Enchanced PPE")]
	public Conversation enchancedPPEselectConversation;
	public Conversation enchancedPPEanimationConversation;



    public bool IsItem()
    {
        return (Item != ITEM.NONE);
    }

    public bool IsAction()
    {
        return (Action != ACTION_TYPE.NONE);
    }

    public bool IsExclusiveCategory()
    {
        return (Category != EXCLUSIVE_CATEGORY.NONE);
    }

    public bool IsOptional()
    {
        return Optional;
    }    
	

    void OnEnable()
    {
        if (HiddenInItemList && (!Automatic && !Optional))
            Debug.Log("- - - WARNING!! BASO " + name + " is set as HiddenInItemList but is neither Automatic nor Optional. You may not be able to equip an entire set that uses this item!");
    }
}









