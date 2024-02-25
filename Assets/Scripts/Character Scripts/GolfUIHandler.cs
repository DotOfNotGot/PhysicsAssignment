using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GolfUIHandler : MonoBehaviour
{
    [SerializeField] private Image forceBar;
    [SerializeField] private Canvas canvas;
    
    public void SetFillAmount(float value)
    {
        forceBar.fillAmount = value;
    }

    public void SetUIActive(bool value)
    {
        SetFillAmount(0);
        canvas.enabled = value;
    }
    
}
