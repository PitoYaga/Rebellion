using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("SecurityCard")] 
    [SerializeField] private GameObject cardUI;
    [SerializeField] private Text securityCardText;
    public int securityCard = 0;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        securityCardText.text = securityCard.ToString();
        
        if (securityCard > 0)
        {
            cardUI.SetActive(true);
        }
        else
        {
            cardUI.SetActive(false);
        }
    }
}
