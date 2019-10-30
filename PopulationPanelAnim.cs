using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopulationPanelAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static PopulationPanelAnim ppa;

    public Animator anim;
    bool Show;
    private GameManager gm;
    // Use this for initialization

   void Awake()
    {
        ppa = this;
    }


    void Start () {      

        Show = anim.GetBool("showPanel");
    }

    public void Toggle()
    {
        Show = !Show;
        if (Show) EventHandler.instance.AddHiddenEvent(EventHandler.EventType.populationClicked); else GameController.instance.canZoomPop = false;
        anim.SetBool("showPanel", Show);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameController.instance.canZoomPop = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameController.instance.canZoomPop = false;
    }





}
