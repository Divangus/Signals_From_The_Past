using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueHangar : MonoBehaviour
{
    public NPCConversation hConversation;
    // Update is called once per frame
    public void StartConversation()
    {
        ConversationManager.Instance.StartConversation(hConversation);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ConversationManager.Instance.PressSelectedOption();
        }
    }
}
