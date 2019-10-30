using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIdragAllow : EventTrigger {

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && PlayerController.instance.previousSelection.selected != PlayerController.Selected.human && PlayerController.instance.currentSelection.selected != PlayerController.Selected.building && PlayerController.instance.currentSelection.selected != PlayerController.Selected.resource && GameController.instance.canMoveHuman && !GameController.instance.panning)
            base.OnBeginDrag(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickTime == 0.1f)
        base.OnPointerClick(eventData);
    }

}
