using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SocialPlatforms.Impl;

public class LoadSceneByIndex : MonoBehaviour
{
    private GameObject manager; // manage the game state
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager");
    }
    // General method to load scenes based on build index
    public void LoadScene(int sceneIndex)
    {
        //Debug.Log("load scene");
        //int maxKill = (sceneIndex == 1) ? 2 : manager.GetComponent<CustomSceneManager>().killLimit;
        //if(CustomSceneManager.instance == null) SceneManager.LoadScene(sceneIndex);
        //else manager.GetComponent<CustomSceneManager>().ResetAndLoad(maxKill+3);

        // we shouldn't be using this anymore, but it is still getting called somewhere.
    }
}