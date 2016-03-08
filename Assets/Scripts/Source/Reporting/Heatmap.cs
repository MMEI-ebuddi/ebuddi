using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class Heatmap : MonoBehaviour {

    public Transform PrefabHeatSpot;

    private List<List<Vector2>> m_ClickPositions;
    private List<int> m_ListIndexes;

    public Canvas HeatmapCanvas;

    private List<GameObject> m_HeatmapPoints;

    private int m_iLastPointIndex = 0;

    private Scene m_BaseScene;

    private int m_iCurrentState = 0;

    public static bool ShowHeatmap = false;

	// Use this for initialization
	void Awake () {
        Init();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowHeatmap = !ShowHeatmap;

            if (ShowHeatmap)
            {
                if (HeatmappingEnabled())
                    ShowPoints(m_iCurrentState);
            }
            else
            {
                HideAllPoints();
            }
        }

        if (!HeatmappingEnabled())
            return;
              
        
	    if (Input.GetMouseButtonDown(0))
        {
            AddClick(m_iCurrentState, new Vector2(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height));
        }
            
        if (m_BaseScene!=null)
        {
            if (m_iCurrentState != m_BaseScene.GetCurrentStateIndex())
            {
                NextState();
            }
        }                
	}

    private void Init()
    {
       m_iLastPointIndex = 0;
       m_BaseScene = GameObject.FindObjectOfType<Scene>();        
       m_ClickPositions = new List<List<Vector2>>();
       m_ListIndexes = new List<int>();
       m_HeatmapPoints = new List<GameObject>();

       LoadPoints();
    }
    
    // Add a new list for the new current state
    public void AddStateList(int iStateIndex)
    {
        if (StateListExists(iStateIndex))
            return;
        
        List<Vector2> newList = new List<Vector2>();

        m_ClickPositions.Add(newList);

        m_ListIndexes.Add(iStateIndex);
    }

    bool StateListExists(int iStateIndex)
    {
        return m_ListIndexes.Contains(iStateIndex);
    }

    public int GetIndexForState(int iState)
    {
        return m_ListIndexes.IndexOf(iState);
    }

    // We've transitioned to the next state
    public void NextState()
    {
        m_iCurrentState = m_BaseScene.GetCurrentStateIndex();

        AddStateList(m_iCurrentState);        
        ShowPoints(m_iCurrentState);
    }

    // A click has been detected
    private void AddClick(int iStateIndex, Vector2 pos)
    {
        if (!StateListExists(iStateIndex))
        {
            Debug.Log("HeatMapper: ERROR! Trying to add a click for an out of bounds state!");
            return;
        }

        AddPoint(iStateIndex, pos);

        ShowPoint(pos);      
    }

    private void AddPoint(int iStateIndex, Vector2 pos)
    {
        if (!StateListExists(iStateIndex))
        {
            Debug.Log("HeatMapper: ERROR! Trying to add a point for an out of bounds state!");
            return;
        }

        List<Vector2> thisStateList = m_ClickPositions[GetIndexForState(iStateIndex)];

        thisStateList.Add(pos);
    }
   
    // Show all points for the specified state, if we have any
    private void ShowPoints(int iStateIndex)
    {
        HideAllPoints();
      
        if (!StateListExists(iStateIndex))
        {
            Debug.Log("HeatMapper: ERROR! Trying to show points for a state index that doesn't exist!");
            return;
        }
        
        foreach (Vector2 pos in m_ClickPositions[GetIndexForState(iStateIndex)])
        {
            ShowPoint(pos);
        }      
    }

    // Shows a heatmap point at the specified point, attempts to reuse old gameobjects or instantiates new if needed
    private void ShowPoint(Vector2 pos)
    {
        if (!ShowHeatmap)
            return;

        Transform newSpot = null;

        // Do we already have a point object to use?
        if (m_iLastPointIndex < m_HeatmapPoints.Count)
        {         
            newSpot = m_HeatmapPoints[m_iLastPointIndex].transform;
            newSpot.gameObject.SetActive(true);
        }
        else
        {         
            newSpot = Instantiate(PrefabHeatSpot) as Transform;
            m_HeatmapPoints.Add(newSpot.gameObject);
        }

        m_iLastPointIndex++;

        newSpot.SetParent(HeatmapCanvas.transform, false);
        newSpot.position = new Vector3(Screen.width * pos.x, Screen.height * pos.y, 0);        
    }

    private void HideAllPoints()
    {
        foreach (GameObject point in m_HeatmapPoints)
        {
            point.SetActive(false);
        }

         m_iLastPointIndex = 0;
    }

    void OnDestroy()
    {
		if (!string.IsNullOrEmpty( UserProfile.sCurrent.Name))
		{        
			SavePoints();
		}
    }

    private bool HeatmappingEnabled()
    {
        return PlayerPrefs.GetInt("HeatmappingEnabled", 0) == 1 ? true : false;
    }

    private void LoadPoints()
    {
        if (!HeatmappingEnabled())
            return;


       
        string strPath = Application.persistentDataPath + "/heatmaps/" + UserProfile.sCurrent.Name + "-" + Application.loadedLevelName + ".xml";

        Debug.Log("Loading heatmap points: " + strPath);

        if (!File.Exists(strPath))
            return;

        Debug.Log("Heatmap file found!");

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(strPath);

        XmlNodeList states = xmlDoc.GetElementsByTagName("state");

        int iCurrentState = -1;

        // Loop through all the "state" sections
        foreach (XmlNode state in states)
        {
            XmlNodeList stateContent = state.ChildNodes;

            // loop through each "state" sub section
            foreach (XmlNode stateItems in stateContent) 
            {
                if (stateItems.Name == "index")
                {                    
                    iCurrentState = int.Parse(stateItems.InnerText);
                    
                    AddStateList(iCurrentState);
                }    
                else if (stateItems.Name == "click")
                {
                    if (iCurrentState!=-1)
                    {                     
                        Vector2 newPoint = new Vector2(float.Parse(stateItems.Attributes["x"].Value), float.Parse(stateItems.Attributes["y"].Value));
                        AddPoint(iCurrentState, newPoint);                        
                    }
                }
            }
        }
    }


    private void SavePoints()
    {
        if (!HeatmappingEnabled())
            return;

        Debug.Log("Saving heatmap points");


        XmlWriterSettings WriterSettings = new XmlWriterSettings();

        WriterSettings.Indent = true;
        WriterSettings.IndentChars = ("\t");
        WriterSettings.OmitXmlDeclaration = true;


		if (!Directory.Exists(Application.persistentDataPath + "/heatmaps/"))
		{
			var folder = Directory.CreateDirectory(Application.persistentDataPath + "/heatmaps/");
			
			if (folder == null)
			{
				Debug.Log("Error creating folder to save heatmaps!");
				return;
			}
		}



        string strPath = Application.persistentDataPath + "/heatmaps/" + UserProfile.sCurrent.Name + "-" + Application.loadedLevelName + ".xml";
        XmlWriter Writer = XmlWriter.Create(strPath, WriterSettings);	


        Writer.WriteStartDocument();        

        int iIndex = 0;

        Writer.WriteStartElement("states");

        foreach (int i in m_ListIndexes)
        {

            Writer.WriteStartElement("state");

            Writer.WriteElementString("index", i.ToString());

            List<Vector2> clicks = m_ClickPositions[iIndex];

            foreach (Vector2 pos in clicks)
            {
                Writer.WriteStartElement("click");
                    Writer.WriteAttributeString("x", pos.x.ToString());
                    Writer.WriteAttributeString("y", pos.y.ToString());                
                Writer.WriteEndElement();
            }

            Writer.WriteEndElement();

            iIndex++;
        }

        Writer.WriteEndElement();

        Writer.WriteEndDocument();

        Writer.Close();

        Debug.Log(strPath);
    }
}
