using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public Scene Ascene;
    void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 60;
        Ascene = SceneManager.GetActiveScene();
    }

    public void LoadScene(string sc)
    {
        SceneManager.LoadScene(sc); // Loads given scene name through inspector
    }

    public void QuitApp()
    {
        Application.Quit(); //Exits Running Process
    }
}
