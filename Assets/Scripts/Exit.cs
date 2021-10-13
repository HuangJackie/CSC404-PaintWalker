using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    private UpdateUI _updateUI;
    public LevelManager manager;
    private AudioSource _winAudioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        _updateUI = FindObjectOfType<UpdateUI>();
        _winAudioSource = FindObjectOfType<AudioSource>();
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        bool playerCollision = collision.gameObject.GetComponent<Collider>().CompareTag("Player");
        if (playerCollision && manager.IsExitActive())
        {
            _updateUI.SetInfoText("You Win!", true);
            Time.timeScale = 0.0f;
            _winAudioSource.Play();
        }
    }
}
