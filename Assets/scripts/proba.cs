using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class proba : MonoBehaviour, IInteractable
{
    Text textTuto;

    // Start is called before the first frame update
    void Start()
    {
        textTuto = GameObject.Find("InteractText").GetComponent<Text>();
        gameObject.GetComponent<Outline>().OnDisable();
    }

    public void NoHover()
    {
        gameObject.GetComponent<Outline>().OnDisable();
        textTuto.enabled = false;
    }

    public void Hover()
    {
        if (!Interaction.isExamining)
        {
            textTuto.enabled = true;
        }
        gameObject.GetComponent<Outline>().OnEnable();
        gameObject.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineVisible;
        gameObject.GetComponent<Outline>().OutlineWidth = 5;
    }
}
