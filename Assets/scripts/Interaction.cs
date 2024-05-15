using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

interface IInteractable
{
    public void Interact();
    public void Hover();
}

public class Interaction : MonoBehaviour
{
    public static bool InputLock = false;
    public Transform InteractorSource;
    public float InteractRange;

    //ofsset for the examine system
    public GameObject offset;

    public static bool isExamining = false;

    private Vector3 lastMousePosition;

    private Transform examinedObject; // Store the currently examined object

    //List of position and rotation of the interactble objects 
    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
        {
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        InputLock = true;
    }

    void StopExamination()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        InputLock = false;
    }

    void Examine()
    {
        if (examinedObject != null)
        {
            examinedObject.position = Vector3.Lerp(examinedObject.position, offset.transform.position, 0.2f);

            Vector3 deltaMouse = Input.mousePosition - lastMousePosition;
            float rotationSpeed = 1.0f;
            examinedObject.Rotate(deltaMouse.x * rotationSpeed * Vector3.up, Space.World);
            examinedObject.Rotate(deltaMouse.y * rotationSpeed * Vector3.left, Space.World);
            lastMousePosition = Input.mousePosition;
        }
    }

    void NonExamine()
    {
        if (examinedObject != null)
        {
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

}
