using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Operate : MonoBehaviour
{
    public float radius = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] hitColliders =
            Physics.OverlapSphere(transform.position, radius);
            foreach (Collider hitCollider in hitColliders)
            {
                hitCollider.SendMessage("TriggerButtton",
                SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
