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
    }

    public void NoHover()
    {
        gameObject.GetComponent<Outline>().enabled = false;
        textTuto.enabled = false;
    }

    public void Hover()
    {
        if (!Interaction.isExamining)
        {
            textTuto.enabled = true;
        }
        gameObject.GetComponent<Outline>().enabled = true;
        gameObject.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineVisible;
        gameObject.GetComponent<Outline>().OutlineColor = Color.yellow;
        gameObject.GetComponent<Outline>().OutlineWidth = 5;
    }
}
