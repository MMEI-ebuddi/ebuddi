using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//namespace TMSupport
//{
	public class UIOptions : MonoBehaviour
	{
		public Toggle			m_Subtitles;
		public Toggle			m_English;
		public Toggle			m_Krio;
        public Toggle           m_Heatmap;

        public GameObject ButtonUploadData;
        public GameObject ButtonViewAnalytics;

        public GameObject SubmittingDataPanel;

        public Text TextSessionCount, TextFileSize, TextProfileCount;

        public Transform PrefabViewAnalytics;

		void Awake()
		{
			m_Subtitles.isOn = false;
			m_English.isOn = MediaNodeManager.Language == eLanguage.English;
			m_Krio.isOn = MediaNodeManager.Language == eLanguage.Krio;
            m_Heatmap.isOn = HeatmappingEnabled();
		}

        void OnEnable()
        {
            AnalyticsData.Instance.DataSubmissionStarted += DataSubmissionStarted;            
            AnalyticsData.Instance.DataSubmissionCompleted += DataSubmissionCompleted;            
            AnalyticsData.Instance.DataSubmissionFailed += DataSubmissionFailed;            

            if (!Application.isEditor) ButtonUploadData.SetActive((UserProfile.sCurrent.Name.ToUpper().Equals("ADMIN")));
			else {
				//always show buttons in editor
				ButtonUploadData.SetActive(true);
			}
            ButtonViewAnalytics.SetActive(ButtonUploadData.active);

            RefreshSessionStats();
        }

        private void RefreshSessionStats()
        {
            TextSessionCount.text = AnalyticsData.GetTotalPendingSessionCount() + " pending session(s).";
            TextFileSize.text = (System.Text.ASCIIEncoding.Unicode.GetByteCount(AnalyticsData.Instance.PostAllUserData(false)).ToString() + " byes of data.");
            TextProfileCount.text = UserProfile.GetUserProfileNames().Count + " profile(s).";
        }

        void OnDisable()
        {
             AnalyticsData.Instance.DataSubmissionStarted -= DataSubmissionStarted;            
        }

        void DataSubmissionStarted()
        {
//            SubmittingDataPanel.SetActive(true);
			if (UILoading.instance != null) UILoading.instance.Show();
            Debug.Log("Data submission started!");
        }

        void DataSubmissionCompleted()
        {
//            SubmittingDataPanel.SetActive(false);

            if (AnalyticsData.ArchiveAllSessions())
                AnalyticsData.DeleteAllSessions();

            RefreshSessionStats();
            Debug.Log("Data submission completed!");
			StartCoroutine(UIModalManager.instance.ShowAlert("Data submission completed", "Analytics data has been submitted successfully", new string[]{"OK"}, delegate(string obj) { }));

			if (UILoading.instance != null) UILoading.instance.Hide();
        }

        void DataSubmissionFailed(string error)
        {
//            SubmittingDataPanel.SetActive(false);
            Debug.LogError("Data submission failed!");
			StartCoroutine(UIModalManager.instance.ShowAlert("Sending data error", error, new string[]{"OK"}, delegate(string obj) { }));
			if (UILoading.instance != null) UILoading.instance.Hide();
        }

		void Update()
		{
			if(m_English.isOn && MediaNodeManager.Language == eLanguage.Krio)
				MediaNodeManager.ChangeLanguage(eLanguage.English);
			else if(m_Krio.isOn && MediaNodeManager.Language == eLanguage.English)
				MediaNodeManager.ChangeLanguage(eLanguage.Krio);

            if (m_Heatmap.isOn != HeatmappingEnabled())
            {
                PlayerPrefs.SetInt("HeatmappingEnabled", m_Heatmap.isOn ? 1 : 0);
            }
		}


        private bool HeatmappingEnabled()
        {
            return (PlayerPrefs.GetInt("HeatmappingEnabled", 0) == 1) ? true : false;
        }

        public void UploadDataButtonClicked()
        {
            Debug.Log("UIOptions::Attempting to post all user data...");

            AnalyticsData.Instance.PostAllUserData();
        }

        public void ViewAnalyticsClicked()
        {
            Debug.Log("Viewing analytics...");

            Instantiate(PrefabViewAnalytics);
        }


		public void SendDataClicked() {
			AnalyticsData.Instance.SendDataByEmail(AnalyticsData.Instance.PostAllUserData(false));
		}

	}
//}
