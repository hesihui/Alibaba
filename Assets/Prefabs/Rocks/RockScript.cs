using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RockScript : MonoBehaviour
{
   public void BlowUp()
    {
        BoxCollider bc;
        if (transform.TryGetComponent<BoxCollider>(out bc))
        {
            bc.enabled = false;
        }
        ApplyRigidBodyAndForce(transform);

        Invoke(nameof(RedrawAStar), 1); //wait a second to redraw the grid because multiple rocks might blow up at once
    }

    public void RedrawAStar()
    {
        GameObject.Find("A*").GetComponent<Grid>().CreateGrid();
    }

    public static void ApplyRigidBodyAndForce(Transform parent)
    {
        foreach (Transform child in parent)
        {
          

            GameObject.Destroy(child.gameObject,1f);

            Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                MeshCollider mc = child.gameObject.AddComponent<MeshCollider>();
                mc.convex = true;
                rb = child.gameObject.AddComponent<Rigidbody>();
            }


            Vector3 randomForce = new Vector3(
                Random.Range(-3f, 3f),
                Random.Range(.5f, 1f),
                Random.Range(-3f, 3f)
            ).normalized * 150;

            rb.AddForce(randomForce, ForceMode.Impulse);


            if (child.position.y < 1)
            {
                Vector3 newPos = child.position;
                newPos.y = 5;
                child.position = newPos;
            }


            ApplyRigidBodyAndForce(child);
        }
    }
}
