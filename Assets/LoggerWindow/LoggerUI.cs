using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class LoggerUI : MonoBehaviour {

    private struct LogMessage
    {
        public string message;
        public string stackTrace;
        public int index; 
        // Could add :log type ; 
    }


    public GameObject               prefabText; // prefab GUI tex 
   


    private  List<LogMessage>       m_logMessages; // all logged messages 
    private List<Text>              m_logObjects; // all object gui texts 
    private int                     m_scrollIndex ; // manual scroll index

    private float                   m_spaceBetweenMessages; // space between two log texts 
    private int                     m_nbDisplayedMessages;  // nb log displayed in the window
    private int                     m_extrimityYWindowY; 

    private void Start()
    {

        m_logObjects = new List<Text>();
        m_logMessages = new List<LogMessage>();
        m_scrollIndex = 0;

        RectTransform rectT = this.GetComponent<RectTransform>();

        m_nbDisplayedMessages = (int) (16 * rectT.sizeDelta.y / 500.0f) ;
        m_spaceBetweenMessages = (float) rectT.sizeDelta.y / (float) m_nbDisplayedMessages;
        m_extrimityYWindowY = ( (int) rectT.sizeDelta.y / 2) - 10;

    }

    void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }



    void UpdateScrollViewAdd()
    {
        // When log added, scroll up all objects
        if (m_logObjects.Count > m_nbDisplayedMessages)
        {
            m_scrollIndex++;
            foreach (Text t in m_logObjects)
            {
                t.rectTransform.localPosition = t.rectTransform.transform.localPosition + new Vector3(0, m_spaceBetweenMessages, 0); 
            }
        }
        UpdateVisibilityTexts(); 

    }

    private void UpdateVisibilityTexts()
    {
        // update visibility off objects in the scroll view 
        foreach (Text text in m_logObjects)
        {
            if (text.transform.localPosition.y > m_extrimityYWindowY || text.transform.localPosition.y < -m_extrimityYWindowY)
            {
                 text.gameObject.SetActive(false); 
            }
            else
            {
                text.gameObject.SetActive(true); 
            }

        }
    }


    void HandleLog(string logString, string stackTrace, LogType type)
    {
        int indexLog = IndexLogMessageIfExist(stackTrace, logString); // check if collapsed log
        if  (indexLog >=0 )
        {
            LogMessage message = m_logMessages[indexLog];
            message.index += 1;
            m_logMessages[indexLog] = message; 
            // update text with collapse number
            m_logObjects[indexLog].text = m_logMessages[indexLog].message + "           " + m_logMessages[indexLog].index; 
            
        }
        else
        {
            // Add new log in log list and create gui text object

            LogMessage message = new LogMessage();
            message.index = 1;
            message.message = logString;
            message.stackTrace = stackTrace;
            m_logMessages.Add(message); 

            GameObject newText = GameObject.Instantiate(prefabText, this.transform.GetChild(0));
            RectTransform transformT = newText.GetComponent<RectTransform>();

            Vector3 positionTxt = new Vector3(this.GetComponent<RectTransform>().offsetMin.x +10 + transformT.sizeDelta.x/2.0f, m_extrimityYWindowY-10, 0);
            if (m_logObjects.Count >0)
            {
                positionTxt = m_logObjects[m_logObjects.Count - 1].transform.localPosition - new Vector3(0, m_spaceBetweenMessages, 0); ; 
            }

            transformT.localPosition = positionTxt;
            transformT.localScale = Vector3.one;

            Text txt = newText.GetComponent<Text>();
            m_logObjects.Add(txt);

            txt.text = logString;
            if (type == LogType.Error)
            {
                txt.color = Color.red;
            }
            else if (type == LogType.Warning)
            {
                txt.color = Color.yellow;
            }
            else if (type == LogType.Log)
            {
                txt.color = Color.white;
            }

            UpdateScrollViewAdd();
        }
    }


    public void UpScrollView()
    {
        // Scroll up the logs

        if (m_logObjects.Count < m_nbDisplayedMessages)
        {
            return; 
        }
        m_scrollIndex -= 1;
        if (m_scrollIndex < 0)
        {
            m_scrollIndex = 0;
            return; 
        }
        foreach (Text t in m_logObjects)
        {
            t.rectTransform.localPosition = t.rectTransform.transform.localPosition - new Vector3(0, m_spaceBetweenMessages, 0);
        }
        UpdateVisibilityTexts(); 
    }

    public void DownScrollView()
    {

        // Scroll down the logs
        if (m_logObjects.Count < m_nbDisplayedMessages)
        {
            return;
        }
        m_scrollIndex += 1;
        if (m_scrollIndex > (m_logObjects.Count-m_nbDisplayedMessages))
        {
            m_scrollIndex= (m_logObjects.Count-m_nbDisplayedMessages);
            return;
        }
        foreach (Text t in m_logObjects)
        {
            t.rectTransform.localPosition = t.rectTransform.transform.localPosition + new Vector3(0, m_spaceBetweenMessages, 0);
        }

        UpdateVisibilityTexts();
    }

	public void ChangeVisibility()
	{
		this.transform.GetChild(0).gameObject.SetActive(!this.transform.GetChild(0).gameObject.activeSelf) ; 
	}


    private int IndexLogMessageIfExist(string trace, string message)
    {
        for (int i = 0; i < m_logMessages.Count; i++)
        {
            // check if collapsed log
            if (trace == m_logMessages[i].stackTrace && message == m_logMessages[i].message)
            {
                return i; 
            }
        }
        return -1; 
    }

    public void Clear()
    {
        m_logMessages.Clear();
        foreach(Text t in m_logObjects)
        {
            Destroy(t.gameObject); 
        }
        m_logObjects.Clear();
        m_scrollIndex = 0; 
    }
}
