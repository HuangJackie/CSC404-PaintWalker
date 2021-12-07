using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private UpdateUI _updateUI;
    public LevelManager manager;
    private AudioSource _winAudioSource;
    private GameObject _player;
    
    // Start is called before the first frame update
    void Start()
    {
        _updateUI = FindObjectOfType<UpdateUI>();
        _winAudioSource = this.GetComponent<AudioSource>();
        _player = GameObject.FindWithTag("Player");
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        Scene scene = SceneManager.GetActiveScene();
        bool playerCollision = collision.gameObject.GetComponent<Collider>().CompareTag("Player");
        if (playerCollision)
        {
            if (scene.name == "TutorialColors")
            {
                _updateUI.SetInfoText("Tutorial Complete!", true);
            }
            else
            {
                _player.SetActive(false);
                _updateUI.SetInfoText("You Win!", true);
                _winAudioSource.Play();
            }
            StartCoroutine(ReturnToMenu());
        }
    }

    private IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(3);
        SceneLoader.LoadNextLevel();
    }
}
