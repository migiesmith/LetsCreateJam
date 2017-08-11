using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HoverController : MonoBehaviour
{

    public UnityEvent hoverStart, hoverEnd;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseEnter()
    {
        hoverStart.Invoke();
    }

    void OnMouseExit()
    {
        hoverEnd.Invoke();
    }

}
