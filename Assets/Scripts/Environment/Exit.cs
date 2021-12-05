using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private UpdateUI _updateUI;
    public LevelManager manager;
    private AudioSource _winAudioSource;
    private RestartDontDeleteManager restartDontDeleteManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _updateUI = FindObjectOfType<UpdateUI>();
        restartDontDeleteManager = FindObjectOfType<RestartDontDeleteManager>();
        _winAudioSource = this.GetComponent<AudioSource>();
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
                _updateUI.SetInfoText("You Win!", true);
                _winAudioSource.Play();
            }
            StartCoroutine(ReturnToMenu());
        }
    }

    private IEnumerator ReturnToMenu()
    {
        restartDontDeleteManager = FindObjectOfType<RestartDontDeleteManager>();
        restartDontDeleteManager.isRestarting = false;
        yield return new WaitForSeconds(3);
        SceneLoader.LoadNextLevel();
    }
}
