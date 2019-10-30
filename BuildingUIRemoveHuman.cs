using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingUIRemoveHuman : MonoBehaviour, IPointerDownHandler
{
    public bool remove;
    public bool remove2;

    public Building building;

    public GameObject humanHere;

    void Start()
    {
   
    }

    private void Update()
    {
        if (building == null)
            building = transform.root.GetComponentInChildren<BuildingController>().building;
        else
        {
            if (building != null)
            {
                if (building.humans.Count > 0)
                {
                    
                }
            }
        }

                /* if (remove || remove2)
                 {
                     Human h = 
                                         //humans[building.HumanIndex(h)] = h;
                                     h._go.transform.position = transform.root.GetComponentInChildren<BuildingController>().doorTransform.position;
                     transform.root.GetComponentInChildren<BuildingController>().RemoveHuman(h);
                     cam[building.HumanIndex(h)] = h._go.GetComponent<HumanController>().cam;
                     cam[building.HumanIndex(h)].gameObject.SetActive(false);
                     cam[building.HumanIndex(h)].targetTexture = null;
                     slots[building.HumanIndex(h)].texture = null;
                     ages[building.HumanIndex(h)].gameObject.SetActive(false);


                 }*/
            }

    public void OnPointerDown(PointerEventData eventData)
    {
       





    }


  
}
