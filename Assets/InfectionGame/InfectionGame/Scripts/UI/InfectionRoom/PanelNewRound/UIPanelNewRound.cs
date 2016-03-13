using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPanelNewRound : MonoBehaviour {

    public RectTransform Panel;

    public Text TextRoundNumber;
    public Text TextRoundDescription;

    public string[] RoundDescriptions;
        
    void Start()
    {
       
    }

    public void StartRoundButtonClicked()
    {
        RoundManager.Instance.StartRound();
        Hide();
    }

	public void Show(int iRound, bool bPPE)
    {
        TextRoundNumber.text = "Round " + (iRound+1).ToString();

        if (iRound == 0)
            TextRoundDescription.text = "- You are unable to take any safety precautions in this round.";
        else
            TextRoundDescription.text = "- Click on infected patients who are in the Triage area to send them to isolation\n\n- Click on a medic or cleaner to make them equip PPE.\n\n- The 3 feet rule is automatically enforced.";

        Panel.gameObject.SetActive(true);
    }

    public void Hide()
    {
        Panel.gameObject.SetActive(false);
    }
}
