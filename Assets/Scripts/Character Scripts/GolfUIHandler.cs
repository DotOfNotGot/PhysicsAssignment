using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GolfUIHandler : MonoBehaviour
{

    [SerializeField] private Image forceBar;
    
    public void SetFillAmount(float value, float maxValue)
    {
        forceBar.fillAmount = value;
    }
    
}
