using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class DialogueManager : MonoBehaviour
{
    public NPCConversation cConversation;
    private bool timerCall = true;
    private float time = 0.0f;
    public AudioClip[] audioClips;
    public AudioSource audios;

    public static bool conversationEnd = false;
    // Start is called before the first frame update

    private void Update()
    {
        if (timerCall)
        {
            if (time > 60.0f)
            {
                ConversationManager.Instance.StartConversation(cConversation);
                audios.clip = audioClips[0];
                audios.Play();
                time = 0;
                timerCall = false;
            }
            if (time > 51.0f && time < 51.1f)
            {
                audios.Play();
                //Debug.Log("RINGGGGGGGGGGGGGGGGGGGGGGGGGGG");
                time += Time.deltaTime;
            }
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
    public void EndConversation()
    {
        conversationEnd = true; 
    }
}
