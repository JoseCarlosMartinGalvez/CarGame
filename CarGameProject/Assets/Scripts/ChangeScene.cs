using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public int sceneNum;
    public int sceneContinue;
    public int sceneMenu;
    public bool Automatic;

    public void changeScene()
    {
        SceneManager.LoadScene(sceneNum);
    }

    public void Update()
    {
        if (!Automatic)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                SceneManager.LoadScene(sceneContinue);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                SceneManager.LoadScene(sceneMenu);
            }
        }
    }
}
