using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class proba : MonoBehaviour, IInteractable
{
    public Text textTuto;
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
