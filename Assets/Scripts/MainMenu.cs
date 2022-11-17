using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject title;
    [SerializeField] GameObject options;
    [SerializeField] GameObject credits;
    [SerializeField] Slider volumeSlider;

    float startTime;


    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        print("Play!");
        EventSystem.current.SetSelectedGameObject(null);
        SceneManager.LoadScene("ChickenGameNew");
    }

    public void QuitGame()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Application.Quit();
    }

    public void OptionsMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        
        title.SetActive(false);
        options.SetActive(true);
    }

    public void Credits()
    {
        EventSystem.current.SetSelectedGameObject(null);

        title.SetActive(false);
        credits.SetActive(true);
    }

    public void BackToTitle()
    {
        EventSystem.current.SetSelectedGameObject(null);

        title.SetActive(true);
        options.SetActive(false);
        credits.SetActive(false);
    }

    public void SetVolume()
    {
        AudioListener.volume = volumeSlider.value;

        //if (Time.time > startTime + 0.5f) {
            GetComponent<AudioSource>().Play();
        //}
    }
}
