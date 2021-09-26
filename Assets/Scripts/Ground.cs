using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public GameObject player;
    public float speed;

    public bool _isPainted;
    private LevelManager _levelManager;
    private Material _material;
    private Color _originalColour;
    private UpdateUI _updateUI;
    
    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponentInChildren<Renderer>().material;
        _originalColour = _material.color;
        _levelManager = FindObjectOfType<LevelManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (!_isPainted && other.gameObject.CompareTag("Player"))
        {
            PaintSurface();
        }
    }
    
    public bool PaintSurface()
    {
        if (_levelManager.HasEnoughPaint() && !_isPainted)
        {
            _levelManager.DecreaseCurrentSelectedPaint(1);
            _isPainted = true;
            string currentlySelectedPaint = _levelManager.GetCurrentlySelectedPaint();
            switch (currentlySelectedPaint)
            {
                case "Red":
                    _material.color = Color.red;
                    _originalColour = _material.color;

                    break;
                case "Green":
                    _material.color = Color.green;
                    _originalColour = _material.color;
                    GreenExtend();
                    break;
                case "Black":
                    _material.color = Color.black;
                    _originalColour = _material.color;
                    BlackFall();
                    break;
                case "Special":
                    _material.color = Color.cyan;
                    _originalColour = _material.color;
                    break;
            }

            return true;
        }

        return false;
    }
    
    private void GreenExtend()
    {

        //extends platforms in wasd directions by 1 block
        Debug.Log(transform.position);
        var growth_block = Resources.Load(path: "Test");
        Instantiate(growth_block, transform.position + Vector3.back,
            Quaternion.identity);
        Instantiate(growth_block, transform.position + Vector3.left,
            Quaternion.identity);
        Instantiate(growth_block, transform.position + Vector3.right,
            Quaternion.identity);
        Instantiate(growth_block, transform.position + Vector3.forward,
            Quaternion.identity);
        //_spawnedBlock.transform.position;

    }

    private void BlackFall()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(0, -1, 0), speed * Time.deltaTime);
    }
}
