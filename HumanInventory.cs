using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanInventory : MonoBehaviour {

    private Building building;
    public GameObject humanBlock;
    private List<GameObject> blocks;

	// Use this for initialization
	void Start () {
        blocks = new List<GameObject>();
	}

    // Update is called once per frame
    void Update()
    {
        if (building == null)
            building = transform.root.GetComponentInChildren<BuildingController>().building;
        else
        {
            if (building != null)
            {
                if (building.humans.Count > 0)
                {
                    foreach (Human h in building.humans)
                    {
                        if (blocks.Count == 0)
                        {
                            GameObject temp = Instantiate(humanBlock, transform);
                            temp.GetComponent<UIHoverCanvas>().human = h;
                            blocks.Add(temp);
                        }
                        else
                        {
                            bool used = false;
                            foreach (GameObject g in blocks)
                            {
                                if (h == g.GetComponent<UIHoverCanvas>().human)
                                {
                                    used = true;
                                }
                            }
                            if (!used)
                            {
                                GameObject temp = Instantiate(humanBlock, transform);
                                temp.GetComponent<UIHoverCanvas>().human = h;
                                blocks.Add(temp);
                            }
                        }
                    }
                }
                int count = 0;
                while (count < blocks.Count)
                {
                    bool Found = false;
                    foreach (Human h in building.humans)
                    {
                        if (h == blocks[count].GetComponent<UIHoverCanvas>().human)
                        {
                            Found = true;
                        }
                    }
                    if (!Found)
                    {
                        Destroy(blocks[count]);
                        blocks.Remove(blocks[count]);
                        return;
                    }
                    count++;
                }
                
            }
        }


        
    }
}
