using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class pushableBlock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        var dir = ReturnDirection(collision.gameObject, this.gameObject);
        if (collision.gameObject.CompareTag("Player"))
        {
            if (dir != Vector3.negativeInfinity)
            {
                if (true)
                {
                    var pos   = transform.position + new Vector3(0, 0.5f, 0) ;
                    if (!Physics.Raycast(pos, dir, maxDistance: 1))
                    {
                        gameObject.transform.Translate(dir); 
                    }
                }
            }
        }
    }
    private Vector3 ReturnDirection( GameObject Object, GameObject ObjectHit )
    {

        Vector3 hitDirection = Vector3.negativeInfinity;
        RaycastHit RayHit;
        Vector3 direction = ( Object.transform.position - ObjectHit.transform.position ).normalized;
        Ray MyRay = new Ray( ObjectHit.transform.position, direction );
         
        if ( Physics.Raycast( MyRay, out RayHit ) ){
                 
            if ( RayHit.collider != null ){
                 
                Vector3 MyNormal = RayHit.normal;
                hitDirection = MyNormal;
            }    
        }

        return hitDirection;
    }
}
