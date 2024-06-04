using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;
using UnityEngine.SceneManagement;
using System;

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

    public AudioSource DoorSource;
    public AudioClip[] audiosDoor;

    public GameObject[] TargetUI;

    public float rotationSensitivity = 2.0f;

    //ofsset for the examine system
    public GameObject offset;

    public static bool isExamining = false;

    private Vector3 lastMousePosition;

    private Transform examinedObject; // Store the currently examined object

    //List of position and rotation of the interactble objects 
    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

    private GameObject pointer;

    RaycastHit hitInfo;

    private bool RedCard, BlueCard = false;

    GameObject[] BlueDoors, RedDoors;

    // Start is called before the first frame update
    void Start()
    {
        pointer = GameObject.Find("Pointer");
        RedDoors = GameObject.FindGameObjectsWithTag("RedDoor");
        BlueDoors = GameObject.FindGameObjectsWithTag("BlueDoor");
    }

    // Update is called once per frame
    void Update()
    {
        HandlePreviousHitObject();        

        if (!isExamining)
        {
            HandleRaycast();
        }
        else
        {
            HandleExaminationInput();
        }

        if (isExamining)
        {
            StartExamination();
            Examine();
            
        }
        else
        {
            StopExamination();
            NonExamine();
        }

        
    }

    void HandlePreviousHitObject()
    {
        if (objectPreviouslyHit != null && objectPreviouslyHit.TryGetComponent(out IInteractable interactObj))
        {
            if(objectPreviouslyHit.CompareTag("RedDoor"))
            {
                foreach(var door in RedDoors)
                {
                    door.GetComponent<IInteractable>().NoHover();
                }
            }
            else if (objectPreviouslyHit.CompareTag("BlueDoor"))
            {
                foreach (var door in BlueDoors)
                {
                    door.GetComponent<IInteractable>().NoHover();
                }
            }
            else
            {
                interactObj.NoHover();
            }
        }
    }

    void HandleExaminationInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isExamining = false;
        }
    }

    void HandleRaycast()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);

        if (Physics.Raycast(r, out hitInfo, InteractRange))
        {
            GameObject hitObject = hitInfo.collider.gameObject;

            if (objectPreviouslyHit != hitObject || objectPreviouslyHit == null)
            {
                objectPreviouslyHit = hitObject;
            }

            if (hitObject.TryGetComponent(out IInteractable interactObj))
            {
                HandleInteraction(interactObj, hitObject);
                CheckCard(interactObj, hitObject);
            }
        }
    }

    private void CheckCard(IInteractable interactObj, GameObject hitObject)
    {

       
    }

    void HandleInteraction(IInteractable interactObj, GameObject hitObject)
    {
        string tag = hitObject.tag;

        switch (tag)
        {
            case "RedCard":
                interactObj.Hover();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hitObject.gameObject.SetActive(false);
                    RedCard = true;
                    TargetUI[0].SetActive(true);
                }
                break;

            case "BlueCard":
                interactObj.Hover();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hitObject.gameObject.SetActive(false);
                    BlueCard = true;
                    TargetUI[1].SetActive(true);
                }
                break;

            case "GreenCard":
                interactObj.Hover();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hitObject.gameObject.SetActive(false);
                    TargetUI[2].SetActive(true);
                }
                break;

            case "StartIncursion":
                if (DialogueManager.conversationEnd)
                {
                    interactObj.Hover();
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        SceneManager.LoadScene("Hangar");
                    }
                }
                break;

            case "RedDoor": case "BlueDoor":
                OpenDoor(tag);
                break;

            default:
                interactObj.Hover();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    isExamining = true;
                    examinedObject = hitInfo.transform;
                    originalPositions[examinedObject] = examinedObject.position;
                    originalRotations[examinedObject] = examinedObject.rotation;
                }
                break;

        }
    }
    //=============================================================================== 

    public void ToggleExamination()
    {
        isExamining = !isExamining;
    }

    void StartExamination()
    {
        lastMousePosition = Input.mousePosition;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pointer.SetActive(false);
        InputLock = true;
    }

    void StopExamination()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pointer.SetActive(true);
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

        float XaxisRotation = -Input.GetAxis("Mouse X");
        float YaxisRotation = Input.GetAxis("Mouse Y");
        if (Input.GetMouseButton(1))
        {
            examinedObject.rotation =
                Quaternion.AngleAxis(XaxisRotation * rotationSensitivity, transform.up) *
                Quaternion.AngleAxis(YaxisRotation * rotationSensitivity, transform.right) *
                examinedObject.rotation;
        }
    }
    void OpenDoor(string DoorColor)
    {
        switch (DoorColor)
        {
            case "RedDoor":
                foreach (var door in RedDoors)
                {
                    door.GetComponent<IInteractable>().Hover();
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (RedCard)
                    {
                        //Open door
                        GameObject.Find("RedDoors").GetComponent<Animator>().SetTrigger("ToggleDoors");
                        DoorSource.clip = audiosDoor[0];
                        DoorSource.Play();  
                    }
                    else
                    {
                        //Play locked audio
                        DoorSource.clip = audiosDoor[1];
                        DoorSource.Play();
                    }                    
                }
                break;
            case "BlueDoor":
                foreach (var door in BlueDoors)
                {
                    door.GetComponent<IInteractable>().Hover();
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (BlueCard)
                    {
                        //Open door
                        GameObject.Find("BlueDoors").GetComponent<Animator>().SetTrigger("ToggleDoors");
                        DoorSource.clip = audiosDoor[0];
                        DoorSource.Play();

                    }
                    else
                    {
                        //Play locked audio
                        DoorSource.clip = audiosDoor[1];
                        DoorSource.Play();
                    }
                }
                break;
            default:
                break;
        }
    }

}
