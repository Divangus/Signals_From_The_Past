using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class DialogueManager : MonoBehaviour
{
    public NPCConversation cConversation;
    private bool timerCall = true;
    private float time = 0.0f; 
    // Start is called before the first frame update
    void Start()
    {
 
    }
    private void Update()
    {
        if (timerCall)
        {
            if (time > 60.0f)
            {
                ConversationManager.Instance.StartConversation(cConversation);
                time = 0;
                timerCall = false;
            }
            //if (time > 50.0f) ;
            else
            {
                //Debug.Log(time);
                time += Time.deltaTime;
            }
        }


        if (ConversationManager.Instance != null)
        {
            UpdateConversationInput();
        }

    }
    void UpdateConversationInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ConversationManager.Instance.PressSelectedOption();
        }
    }
}
