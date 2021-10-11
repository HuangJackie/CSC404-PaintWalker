using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PushableBlock : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var dir = ReturnDirection(collision.gameObject, this.gameObject);
        if (collision.gameObject.CompareTag("Player"))
        {
            if (dir != Vector3.negativeInfinity)
            {
                var pos = transform.position + new Vector3(0, 0.5f, 0) ;
                if (!Physics.Raycast(pos, dir, maxDistance: 1))
                {
                    gameObject.transform.Translate(dir); 
                }
            }
        }
    }

    private Vector3 ReturnDirection( GameObject Object, GameObject ObjectHit )
    {
        Vector3 hitDirection = Vector3.negativeInfinity;
        RaycastHit RayHit;
        Vector3 direction = (Object.transform.position - ObjectHit.transform.position).normalized;
        Ray MyRay = new Ray(ObjectHit.transform.position, direction);
         
        if ( Physics.Raycast( MyRay, out RayHit ) ){
            if ( RayHit.collider != null ){
                Vector3 MyNormal = RayHit.normal;
                hitDirection = MyNormal;
            }    
        }

        return hitDirection;
    }
}
