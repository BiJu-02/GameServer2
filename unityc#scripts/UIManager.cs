using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject StartMenu;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("UI Instance already exists");
            Destroy(Instance);
        }
    }

    public void ConnectToServer()
    {

        Client.Instance.ConnectToServer();
        if (Client.Instance.IsConnected)
        {
            SceneManager.LoadScene("Queue");
        }
        else
        {
            Debug.Log("ehe...");
        }
    }


    public void Disconnected()
    {
        SceneManager.LoadScene("Main");
    }
}
