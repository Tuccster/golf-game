using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMeshes : MonoBehaviour
{
	public bool _combineOnStart = true;

    private void Start()
    {
        CombineMeshesInChildren();
    }

    public void CombineMeshesInChildren()
    {
		if (!_combineOnStart) return;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

		for (int i = 0; i < meshFilters.Length; i++)
		{
			combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
		}

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.GetComponent<MeshCollider>().sharedMesh = new Mesh();
        transform.GetComponent<MeshCollider>().sharedMesh.CombineMeshes(combine);

		for (int j = 0; j < transform.childCount; j++)
		{
			transform.GetChild(j).gameObject.SetActive(true);
			foreach (Collider collider in transform.GetChild(j).GetComponents<Collider>())
			{
				Destroy(collider);
			}
		}

		gameObject.SetActive(true);
    }
}
