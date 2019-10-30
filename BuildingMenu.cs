using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour
{

    public GameObject BuildingButton;
    public RectTransform Group;
    public List<GameObject> buttons;

    // Use this for initialization
    void Start()
    {
        foreach (BuildingObject o in GameController.instance.buildingObjects)
        {
            GameObject button = Instantiate(BuildingButton, Group);
            BuildingButton b = button.GetComponent<BuildingButton>();
            b.buildingObject = o;
            buttons.Add(button);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.instance.canBuildingList)
        {
            if (!transform.GetChild(0).gameObject.activeInHierarchy)
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }
        } else
        {
            if (transform.GetChild(0).gameObject.activeInHierarchy)
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }


}
