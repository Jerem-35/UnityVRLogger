using UnityEngine;
using System.Collections;

public class RayInteraction  : MonoBehaviour
{

    /// <summary>
    /// Delegate for object pointing 
    /// </summary>
    /// <param name="obj"> object pointed </param>
    public delegate void onPointObject(Object obj);
    /// <summary>
    ///  Register to get when an object is pointed
    /// </summary>
    public onPointObject onPointObjectDelegate;
    /// <summary>
    /// Delegate for object unpointing 
    /// </summary>
    public delegate void onUnPointObject(Object obj);
    /// <summary>
    ///  Register to get when an object is unpointed
    /// </summary>
    /// <param name="obj"> object unpointed </param>
    public onPointObject onUnPointObjectDelegate;
    /* Delegate appelés respectivement lors de la sélection et la deselection d'un objet */
    /// <summary>
    ///  Delegate for object selection
    /// </summary>
    /// <param name="obj"> object selected </param>
    public delegate void onSelectObject(Object obj);
    /// <summary>
    /// Object selection object
    /// </summary>
    public onSelectObject onSelectObjetDelegate;
    /// <summary>
    /// Delegate for object unselection
    /// </summary>
    /// <param name="obj"> object unselected </param>
    public delegate void onUnSelectObject(Object obj);
    /// <summary>
    /// Event unselection object
    /// </summary>
    public onUnSelectObject onUnselectObjectDelegate;





    /// <summary>
    /// Delegate change ray visu state
    /// </summary>
    public delegate void onChangeVisuState();
    /// <summary>
    /// event change visu state
    /// </summary>
    public onChangeVisuState onChangeVisuStateDelegate;







    Transform                       m_rayScale;
    float                           m_trueLength; 
    float                           m_physicalLength;
    GameObject                      m_selectedObject;
    GameObject                      m_pointedObject;
    bool                            m_previousTrigger;


    public  void Start()
    {
      
        m_physicalLength = 100.0f;
        m_trueLength = 100.0f; 
        createGeometry(); 
        m_selectedObject = null;
        m_pointedObject = null;
        m_previousTrigger = false; 

    }


    public  void Update()
    {

        m_trueLength = Mathf.Max(0, m_trueLength);
        m_physicalLength = Mathf.Max(0, m_physicalLength); 
        RaycastHit hitInfo;
        Ray rayon = new Ray(this.transform.position, this.transform.forward);

        bool triggerPressed = SteamVR_Controller.Input((int)this.GetComponent<SteamVR_TrackedObject>().index).GetPress(SteamVR_Controller.ButtonMask.Trigger);

        bool pressDown = triggerPressed && !m_previousTrigger;
        m_previousTrigger = triggerPressed;
       
        if (m_selectedObject!=null && !triggerPressed)
        {
            unSelectObject(); 
        }
        if (m_physicalLength > 0 && Physics.Raycast(rayon, out hitInfo, m_trueLength))
        {

            if (m_pointedObject != null && m_pointedObject != hitInfo.collider.gameObject)
            {
                unPointObject(); 
            }
            if (m_pointedObject == null)
            {
                pointObject(hitInfo.collider.gameObject); 
            }
            m_physicalLength = hitInfo.distance;
            if (m_selectedObject==null)
           {
               if (pressDown)
               {
                   selectObject(hitInfo.collider.gameObject, hitInfo.point);
               }
           } 
        }
        else
        {
            if (m_pointedObject != null)
            {
                unPointObject();
            }

            if (m_selectedObject == null)
            {
                m_physicalLength = m_trueLength;
            }
        }
        m_rayScale.localScale = new Vector3(1, m_physicalLength / 2.0f, 1);

    }

    public void unPointObject()
    {
        if (onUnPointObjectDelegate!= null)
        {
            onUnPointObjectDelegate(m_pointedObject);
        }
        m_pointedObject = null;
       
    }

    public void pointObject(GameObject obj)
    {
        m_pointedObject = obj;
        if (onPointObjectDelegate != null)
        {
            onPointObjectDelegate(obj);
        }
    }

    public void unSelectObject()
    {
        if (onUnselectObjectDelegate != null)
        {
            onUnselectObjectDelegate(m_selectedObject);
        }
        m_selectedObject = null; 
    }

    public void selectObject(GameObject obj, Vector3 point)
    {
        m_selectedObject = obj;
        if (onSelectObjetDelegate != null)
        {
            onSelectObjetDelegate(obj);
        }
    }


  



    protected  void createGeometry()
    {

        // Ray geometry
        GameObject rotationScaleRay = new GameObject();
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.GetComponent<Collider>().enabled = false;
        cylinder.GetComponent<MeshRenderer>().material.color = Color.red; 

        rotationScaleRay.transform.parent = this.transform;
        cylinder.transform.parent = rotationScaleRay.transform;

        rotationScaleRay.transform.localRotation = Quaternion.Euler(90, 0, 0);
        rotationScaleRay.transform.localScale = new Vector3(1, 1.0f / 2.0f, 1);
        m_rayScale = rotationScaleRay.transform;

        rotationScaleRay.transform.localPosition = Vector3.zero;
        m_rayScale.transform.localPosition = Vector3.zero; 

        cylinder.transform.localScale = new Vector3(0.003f, 1, 0.003f);
        cylinder.transform.localPosition = new Vector3(0, 1, 0);


        GameObject child = new GameObject();
        child.transform.parent = this.transform;

    }




}
