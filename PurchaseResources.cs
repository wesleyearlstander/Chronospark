using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PurchaseResources : MonoBehaviour, IPointerDownHandler
{
    public BuildingButton bb;
    public Button cancelButton;
    public bool cancel;

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerController.instance.ConstructionClick(bb.buildingObject);
        cancel = true;
            cancelButton.gameObject.SetActive(true);
            BuildingButton.instance.purchaseButton.gameObject.SetActive(false);
            BuildingButton.instance.notEnoughButton.gameObject.SetActive(false);
        

    }


}
