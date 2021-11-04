using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public GameObject Exit;
    private Renderer _exitRenderer;

    private Material _activeExitMaterial;
    public LevelManager manager;

    void Start()
    {
        _exitRenderer = Exit.GetComponent<Renderer>();
        _activeExitMaterial = Resources.Load("Materials/ActiveExit", typeof(Material)) as Material;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<Collider>().CompareTag("Ground"))
        {
            _exitRenderer.material = _activeExitMaterial;
            manager.SetExitActive(true);
        }
    }
}