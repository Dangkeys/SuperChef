using System;
using UnityEngine;
using Zenject;

public class SettingsHandler : MonoBehaviour
{
    [SerializeField] private GameObject PauseUIGameObject; // refactor this to use settingsUI
    private GameInputReader inputReader;
    private bool isOpenSettings = false; //TODO: change this later it should check to pauseui enable instead of using flag like this to keep single place of truth
    [Inject]
    private void Init(GameInputReader gameInputReader)
    {
        inputReader = gameInputReader;
    }
    void Start()
    {
        inputReader.OpenSettingEvent += ToggleSettingsUI;
    }

    private void ToggleSettingsUI()
    {
        isOpenSettings = !isOpenSettings;
        PauseUIGameObject.SetActive(isOpenSettings);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnDestroy()
    {

    }
}
