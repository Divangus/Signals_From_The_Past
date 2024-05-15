using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proba : MonoBehaviour, IInteractable
{
   public void Interact()
    {
        Debug.Log(Random.Range(0, 100));
    }

    public void Hover()
    {
        gameObject.GetComponent<Outline>().enabled = true;
        gameObject.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineVisible;
        gameObject.GetComponent<Outline>().OutlineColor = Color.yellow;
        gameObject.GetComponent<Outline>().OutlineWidth = 5;
    }

    void Update()
    {
        gameObject.GetComponent<Outline>().enabled = false;
    }
}
