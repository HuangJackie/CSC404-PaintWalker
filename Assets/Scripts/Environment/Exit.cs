using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private UpdateUI _updateUI;
    public LevelManager manager;
    private AudioSource _winAudioSource;
    private CutSceneManager _cutSceneManager;
    private PauseMenu _pauseMenu;
    private RestartDontDeleteManager restartDontDeleteManager;
    private GameObject _player;
    
    // Start is called before the first frame update
    void Start()
    {
        _updateUI = FindObjectOfType<UpdateUI>();
        restartDontDeleteManager = FindObjectOfType<RestartDontDeleteManager>();
        _winAudioSource = this.GetComponent<AudioSource>();
        _cutSceneManager = FindObjectOfType<CutSceneManager>();
        _pauseMenu = FindObjectOfType<PauseMenu>();
        _player = GameObject.FindWithTag("Player");
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        Scene scene = SceneManager.GetActiveScene();
        bool playerCollision = collision.gameObject.GetComponent<Collider>().CompareTag("Player");
        if (playerCollision)
        {
            if (scene.name == "TutorialColors" || scene.name == "Tutorial1" || scene.name == "Tutorial2" || scene.name == "Tutorial15")
            {
                _updateUI.SetInfoText("Tutorial Complete!", true);
            }
            else if (scene.name == "Level3")
            {
                _player.SetActive(false);
                _updateUI.SetInfoText("You Win!", true);
                _winAudioSource.Play();
            }
            StartCoroutine(ReturnToMenu(scene));
        }
    }

    private IEnumerator ReturnToMenu(Scene scene)
    {
        restartDontDeleteManager = FindObjectOfType<RestartDontDeleteManager>();
        restartDontDeleteManager.isRestarting = false;
        yield return new WaitForSeconds(1);
        if (scene.name == "Level3")
        {
            _cutSceneManager.TriggerEndCutScene();
        }
        while (_cutSceneManager.gameObject.activeSelf)
        {
            yield return null;
        }
        _pauseMenu.FadeOut();
    }
}
