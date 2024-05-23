using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

interface IInteractable
{
    public void NoHover();
    public void Hover();
}

public class Interaction : MonoBehaviour
{
    public GameObject player;
    private GameObject objectPreviouslyHit;
    public static bool InputLock = false;
    public Transform InteractorSource;
    public float InteractRange;

    public float rotationSensitivity = 2.0f;

    //ofsset for the examine system
    public GameObject offset;

    public static bool isExamining = false;

    bool canDrop = true;

    private Vector3 lastMousePosition;

    private Transform examinedObject; // Store the currently examined object

    //List of position and rotation of the interactble objects 
    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

    RaycastHit hitInfo;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (objectPreviouslyHit != null)
        {
            if(objectPreviouslyHit.TryGetComponent(out IInteractable interactObj))
            {
                interactObj.NoHover();
            }
        }
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);

        if (Physics.Raycast(r, out hitInfo, InteractRange))
        {
            if (objectPreviouslyHit != hitInfo.collider.gameObject || objectPreviouslyHit == null)
            {
                objectPreviouslyHit = hitInfo.collider.gameObject;
            }

            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                interactObj.Hover();

                if (Input.GetKeyDown(KeyCode.E)) 
                {
                    ToggleExamination();

                    if (isExamining)
                    {
                        examinedObject = hitInfo.transform;
                        originalPositions[examinedObject] = examinedObject.position;
                        originalRotations[examinedObject] = examinedObject.rotation;
                    }

                }
            }

        }

        if (isExamining)
        {
            Examine(); StartExamination();
            //if (Input.GetKeyDown(KeyCode.E)) { ToggleExamination(); }

        }
        else
        {
            NonExamine(); StopExamination();
        }
    }


    public void ToggleExamination()
    {
        isExamining = !isExamining;

    }

    void StartExamination()
    {

        lastMousePosition = Input.mousePosition;
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
        InputLock = true;
    }

    void StopExamination()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        InputLock = false;
    }

    void Examine()
    {
        if (examinedObject != null)
        {
            examinedObject.position = Vector3.Lerp(examinedObject.position, offset.transform.position, 0.2f);

            //make sure object doesnt collide with player, it can cause weird bugs
            Physics.IgnoreCollision(examinedObject.GetComponent<Collider>(), player.GetComponent<Collider>(), true);

            RotateObject();
            lastMousePosition = Input.mousePosition;
        }
    }

    void NonExamine()
    {
        if (examinedObject != null)
        {
            //re-enable collision with player
            Physics.IgnoreCollision(examinedObject.GetComponent<Collider>(), player.GetComponent<Collider>(), false);

            // Reset the position and rotation of the examined object to its original values
            if (originalPositions.ContainsKey(examinedObject))
            {
                examinedObject.position = Vector3.Lerp(examinedObject.position, originalPositions[examinedObject], 0.2f);
            }
            if (originalRotations.ContainsKey(examinedObject))
            {
                examinedObject.rotation = Quaternion.Slerp(examinedObject.rotation, originalRotations[examinedObject], 0.2f);
            }
        }
    }

    void RotateObject()
    {
        if (Input.GetKey(KeyCode.R))//hold R key to rotate, change this to whatever key you want
        {
            canDrop = false; //make sure throwing can't occur during rotating

            //disable player being able to look around
            //mouseLookScript.verticalSensitivity = 0f;
            //mouseLookScript.lateralSensitivity = 0f;
            

            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;
            //rotate the object depending on mouse X-Y Axis
            examinedObject.transform.Rotate(Vector3.down, XaxisRotation);
            examinedObject.transform.Rotate(Vector3.right, YaxisRotation);
        }
        else
        {
            //re-enable player being able to look around
            //mouseLookScript.verticalSensitivity = originalvalue;
            //mouseLookScript.lateralSensitivity = originalvalue;
            canDrop = true;
        }
    }

}
