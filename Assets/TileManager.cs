using UnityEngine;
using System.Collections;

public class TileManager : MonoBehaviour {

    public Material raised;

	// Update is called once per frame
	void Update ()
    {
	    if (Input.GetMouseButtonDown(0))
        {
            Vector3 omousepos = Input.mousePosition;
            omousepos.z = 7f;
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(omousepos);
            //GameObject cursor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cursor.transform.position = mousepos;
            RaycastHit hit;
            if (Physics.Raycast(mousepos, Camera.main.transform.forward, out hit, 30f))
            {
                Debug.Log(mousepos);
                if (hit.collider.tag == "tile")
                {
                    if (hit.collider.transform.localPosition.z == -0.5f)
                    { hit.collider.transform.localPosition -= Vector3.forward; }
                    else
                    { hit.collider.transform.localPosition += Vector3.forward; }
                }
            }
            else
            { Debug.Log("This is useless"); }
        }
	}
}
