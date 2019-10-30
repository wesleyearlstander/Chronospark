using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CancelButton : MonoBehaviour, IPointerDownHandler
{
    public PurchaseResources pr;

    public void Update()
    {
        if (PlayerController.instance.placingConstruct == false)
        {
           this.gameObject.SetActive(false);
            pr.cancel = false;

        }
        else if (PlayerController.instance.placingConstruct == true)
        {
           
          //      this.gameObject.SetActive(true);
                BuildingButton.instance.purchaseButton.gameObject.SetActive(false);
                BuildingButton.instance.notEnoughButton.gameObject.SetActive(false);
            pr.cancel = true;
            
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerController.instance.ConstructionCancel();
        pr.cancel = false;
    }

    }
