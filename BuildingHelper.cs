using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHelper : MonoBehaviour {

    public Material hologram, HologramRed;
    [HideInInspector]
    public List<GameObject> gos;
    [HideInInspector]
    public List<MeshRenderer> mrs;

    public bool canPlace = true;

	// Use this for initialization
	void Start () {
        gos = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if (gos.Count > 0 || (GameController.instance.GetComponent<MovieController>().isActiveAndEnabled && (transform.position - GameController.instance.placementSpot).magnitude > 3))
        {
            if (mrs[0].materials[0] != HologramRed)
                foreach (MeshRenderer m in mrs)
                {
                    Material[] mat = new Material[m.materials.Length];
                    for (int i = 0; i < m.materials.Length; i++)
                    {
                        mat[i] = HologramRed;
                    }
                    m.materials = mat;
                    canPlace = false;
                }
        } else
        {
            if (mrs[0].materials[0] != hologram)
                foreach (MeshRenderer m in mrs)
                {
                    Material[] mat = new Material[m.materials.Length];
                    for(int i = 0; i < m.materials.Length; i++)
                    {
                        mat[i] = hologram;
                    }
                    m.materials = mat;
                    canPlace = true;
                }
        }
	}
}
