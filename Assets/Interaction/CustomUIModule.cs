using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CustomUIModule : BaseInputModule
{

    private GameObject targetObject;
    private bool isSelection;
    private bool isFirstSelection;
    public RayInteraction selectionTool ; 
    void Awake()
    {
        isSelection = false; 
    }

	void Start()
	{
        selectionTool.onSelectObjetDelegate += SelectObject;
        selectionTool.onUnselectObjectDelegate += UnSelectObject;
        selectionTool.onPointObjectDelegate += PointObject;
        selectionTool.onUnPointObjectDelegate += UnPointObject; 
	}

    private bool isPaused; 


    void OnApplicationFocus(bool hasFocus)
    {
        isPaused = !hasFocus;
    }


    void Update()
    {
       if (isPaused)
        {
            this.Process(); 
        }

    }

    private bool SendUpdateEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return false;
        BaseEventData data = GetBaseEventData();
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
        return data.used;
    }

    public override void Process()
    {

        bool usedEvent = SendUpdateEventToSelectedObject();

        if (eventSystem.sendNavigationEvents)
        {

            if (!usedEvent)
            {
           
                if (eventSystem.currentSelectedGameObject == null)
                {
                }
                else
                {
                    if (isFirstSelection)
                    {
                        BaseEventData data = GetBaseEventData();
                        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);
                        isFirstSelection = false;
                    }
                    else
                    {
                        ExecuteEvents.GetEventHandler<IPointerClickHandler>(eventSystem.currentSelectedGameObject);
                    }
                }
            }
        } 
    }
  
    private void SelectObject(System.Object obj)
    {
        GameObject trueObj = (GameObject)obj;
        isSelection = true;
        isFirstSelection = true;
        eventSystem.SetSelectedGameObject(trueObj);
        BaseEventData data = GetBaseEventData();
    }

    private void exitPointObject(GameObject obj)
    {

        PointerEventData pEvent = new PointerEventData(this.eventSystem);
        pEvent.worldPosition = obj.transform.position;
        ExecuteEvents.Execute(obj, pEvent, ExecuteEvents.pointerExitHandler);
    }

    private void PointObject(System.Object obj)
    {
        GameObject trueObj = (GameObject)obj;
        PointerEventData pEvent = new PointerEventData(this.eventSystem);
        pEvent.pointerEnter = trueObj;
        pEvent.worldPosition = trueObj.transform.position;
        ExecuteEvents.Execute(trueObj, pEvent, ExecuteEvents.pointerEnterHandler);
    }

    private void UnSelectObject(System.Object obj)
    {
        GameObject trueObj = (GameObject)obj;
        exitPointObject(trueObj) ; 
        isSelection = false;
        eventSystem.SetSelectedGameObject(null);
    }

    private void UnPointObject(System.Object obj)
    {
        GameObject trueObj = (GameObject)obj;
        exitPointObject(trueObj);
    }


}