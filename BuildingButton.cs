using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public Text title;
    public Text status;
    public Image background;
    public Image icon;
    public BuildingObject buildingObject;
    public Text description;
    public Button purchaseButton;
    public Button notEnoughButton;
    public Button cancelButton;
    public Image resourceIcon;
    public bool showButtons;
    public PurchaseResources pr;

	public Text peopleAmountForShrine;
	public Text achivementAmountForShrine;
	public Image achivementShrine;
	public Image peopleShrine;

    public static BuildingButton instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (buildingObject != null)
        {
            title.text = buildingObject.name;
			if (GameController.instance.gameManager.resourceManager.EnoughResources (buildingObject.resourceCost)) {
				if (buildingObject.name == "Shrine") {
					if (GameController.instance.gameManager.humanManager._currentPopulation >= 15 && EventHandler.instance.currentAchievements.Count >= 7) {

						status.text = buildingObject.resourceCost [0].ToString ();
						description.text = buildingObject.description;
						resourceIcon.sprite = buildingObject.requiredResourceIcon;

						if (showButtons == true) {
							peopleAmountForShrine.gameObject.SetActive (true);
							peopleShrine.gameObject.SetActive (true);
							achivementShrine.gameObject.SetActive (true);
							achivementAmountForShrine.gameObject.SetActive (true);

							if (pr.cancel == false) {
								purchaseButton.gameObject.SetActive (true);
								notEnoughButton.gameObject.SetActive (false);
							} else
								cancelButton.gameObject.SetActive (true);

						} else if (showButtons == false) {

							purchaseButton.gameObject.SetActive (false);
							notEnoughButton.gameObject.SetActive (false);
							cancelButton.gameObject.SetActive (false);
							peopleAmountForShrine.gameObject.SetActive (false);
							peopleShrine.gameObject.SetActive (false);
							achivementShrine.gameObject.SetActive (false);
							achivementAmountForShrine.gameObject.SetActive (false);

						}
					} else {
						status.text = buildingObject.resourceCost[0].ToString();
						description.text = buildingObject.description;
						resourceIcon.sprite = buildingObject.requiredResourceIcon;

						if (showButtons == true)
						{
							peopleAmountForShrine.gameObject.SetActive (true);
							peopleShrine.gameObject.SetActive (true);
							achivementShrine.gameObject.SetActive (true);
							achivementAmountForShrine.gameObject.SetActive (true);

							if (pr.cancel != true)
							{
								purchaseButton.gameObject.SetActive(false);
								notEnoughButton.gameObject.SetActive(true);
							}
							else
								cancelButton.gameObject.SetActive(true);


						}
						else if (showButtons == false)
						{
							purchaseButton.gameObject.SetActive(false);
							notEnoughButton.gameObject.SetActive(false);
							cancelButton.gameObject.SetActive(false);
							peopleAmountForShrine.gameObject.SetActive (false);
							peopleShrine.gameObject.SetActive (false);
							achivementShrine.gameObject.SetActive (false);
							achivementAmountForShrine.gameObject.SetActive (false);


						}
					}



				} else {
                    if (buildingObject.MainBuilding.GetComponent<ResourceController>() != null) { 

                        if (GameController.instance.GetComponent<MovieController>().isActiveAndEnabled) {
                            if (GameController.instance.gameManager.resourceManager.crops < 2)
                            {
                                status.text = buildingObject.resourceCost[0].ToString();
                                description.text = buildingObject.description;
                                resourceIcon.sprite = buildingObject.requiredResourceIcon;

                                if (showButtons == true)
                                {
                                    if (pr.cancel == false)
                                    {
                                        purchaseButton.gameObject.SetActive(true);
                                        notEnoughButton.gameObject.SetActive(false);
                                    }
                                    else
                                        cancelButton.gameObject.SetActive(true);

                                }
                                else if (showButtons == false)
                                {

                                    purchaseButton.gameObject.SetActive(false);
                                    notEnoughButton.gameObject.SetActive(false);
                                    cancelButton.gameObject.SetActive(false);

                                }
                            } else
                            {
                                status.text = buildingObject.resourceCost[0].ToString();
                                description.text = buildingObject.description;
                                resourceIcon.sprite = buildingObject.requiredResourceIcon;

                                if (showButtons == true)
                                {
                                    if (pr.cancel != true)
                                    {
                                        purchaseButton.gameObject.SetActive(false);
                                        notEnoughButton.gameObject.SetActive(true);
                                    }
                                    else
                                        cancelButton.gameObject.SetActive(true);


                                }
                                else if (showButtons == false)
                                {
                                    purchaseButton.gameObject.SetActive(false);
                                    notEnoughButton.gameObject.SetActive(false);
                                    cancelButton.gameObject.SetActive(false);

                                }
                            }
                        } else
                        {
                            status.text = buildingObject.resourceCost[0].ToString();
                            description.text = buildingObject.description;
                            resourceIcon.sprite = buildingObject.requiredResourceIcon;

                            if (showButtons == true)
                            {
                                if (pr.cancel == false)
                                {
                                    purchaseButton.gameObject.SetActive(true);
                                    notEnoughButton.gameObject.SetActive(false);
                                }
                                else
                                    cancelButton.gameObject.SetActive(true);

                            }
                            else if (showButtons == false)
                            {

                                purchaseButton.gameObject.SetActive(false);
                                notEnoughButton.gameObject.SetActive(false);
                                cancelButton.gameObject.SetActive(false);

                            }
                        }
                    }
                    else
                    {
                        status.text = buildingObject.resourceCost[0].ToString();
                        description.text = buildingObject.description;
                        resourceIcon.sprite = buildingObject.requiredResourceIcon;

                        if (showButtons == true)
                        {
                            if (pr.cancel == false)
                            {
                                purchaseButton.gameObject.SetActive(true);
                                notEnoughButton.gameObject.SetActive(false);
                            }
                            else
                                cancelButton.gameObject.SetActive(true);

                        }
                        else if (showButtons == false)
                        {

                            purchaseButton.gameObject.SetActive(false);
                            notEnoughButton.gameObject.SetActive(false);
                            cancelButton.gameObject.SetActive(false);

                        }
                    }

				}
					


			} else {
				if (buildingObject.name == "Shrine") {
				
					status.text = buildingObject.resourceCost [0].ToString ();
					description.text = buildingObject.description;
					resourceIcon.sprite = buildingObject.requiredResourceIcon;

					if (showButtons == true) {
						peopleAmountForShrine.gameObject.SetActive (true);
						peopleShrine.gameObject.SetActive (true);
						achivementShrine.gameObject.SetActive (true);
						achivementAmountForShrine.gameObject.SetActive (true);
						
						if (pr.cancel != true) {
							purchaseButton.gameObject.SetActive (false);
							notEnoughButton.gameObject.SetActive (true);
						} else
							cancelButton.gameObject.SetActive (true);


					} else if (showButtons == false) {
						purchaseButton.gameObject.SetActive (false);
						notEnoughButton.gameObject.SetActive (false);
						cancelButton.gameObject.SetActive (false);
						peopleAmountForShrine.gameObject.SetActive (false);
						peopleShrine.gameObject.SetActive (false);
						achivementShrine.gameObject.SetActive (false);
						achivementAmountForShrine.gameObject.SetActive (false);


					}
				
				} else {


					status.text = buildingObject.resourceCost [0].ToString ();
					description.text = buildingObject.description;
					resourceIcon.sprite = buildingObject.requiredResourceIcon;

					if (showButtons == true) {
						if (pr.cancel != true) {
							purchaseButton.gameObject.SetActive (false);
							notEnoughButton.gameObject.SetActive (true);
						} else
							cancelButton.gameObject.SetActive (true);
                    

					} else if (showButtons == false) {
						purchaseButton.gameObject.SetActive (false);
						notEnoughButton.gameObject.SetActive (false);
						cancelButton.gameObject.SetActive (false);

					}
                
				}
				
			}
            icon.sprite = buildingObject.icon;
        }
    }


    public void Show()
    {
        showButtons = true;
    }

    public void DontShow()
    {
        showButtons = false;
    }

    }
