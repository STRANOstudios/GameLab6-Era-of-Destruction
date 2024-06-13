using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent, RequireComponent(typeof(GameManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        #region Singleton

        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        #endregion
    }
}
