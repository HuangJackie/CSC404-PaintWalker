using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public GameObject Exit;
    private Renderer _exitRenderer;

    private Material _activeExitMaterial;
    // private Color _originalColour;

    // Start is called before the first frame update
    void Start()
    {
        _exitRenderer = Exit.GetComponent<Renderer>();
        // _originalColour = _material.color;
        _activeExitMaterial = Resources.Load("ActiveExit", typeof(Material)) as Material;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Entered Pressure Plate");
        if (collision.gameObject.GetComponent<Collider>().CompareTag("Ground"))
        {
            Debug.Log("Ground Pressure Plate Collision");

            _exitRenderer.material = _activeExitMaterial;
            // _originalColour = _material.color;
        }
    }
}