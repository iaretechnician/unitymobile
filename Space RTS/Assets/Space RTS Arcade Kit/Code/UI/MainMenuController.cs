using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;

/// <summary>
/// Handles input on the Main Menu scene.
/// </summary>
public class MainMenuController : MonoBehaviour {

    private int selectedItem = 0;

    public GameObject[] MenuItems;
    private Text[] _menuTextComponents;

    public AudioClip ScrollSound, ConfirmSound;
    private AudioSource _audioSource;

    public GameObject NewGameDialog, ContinueGameDialog, QuitGameDialog;
    private bool _newGameClicked, _continueClicked, _quitClicked;

    // Camera animation parameters
    public AnimationCurve curve;
    public static bool IS_ACTIVE = false;
    public Transform ProfilePosition, MainPosition;
    private float _timer;
    private static float CAMERA_SHIFT_TIME = 2f;

    private void Awake()
    {
        _menuTextComponents = new Text[MenuItems.Length];
        for (int i = 0; i < MenuItems.Length; i++)
        {
            _menuTextComponents[i] = MenuItems[i].GetComponentInChildren<Text>();
        }

        EventManager.PointerEntry += OnPointerEntry;

        _menuTextComponents[selectedItem].color = Color.red;
        MenuItems[selectedItem].transform.localScale = Vector3.one * 1.1f;
    }

    private void OnDestroy()
    { 
        EventManager.PointerEntry -= OnPointerEntry;
    }

    private void OnPointerEntry(object sender, EventArgs e)
    {
        if ((((GameObject)sender).GetComponent<ClickableText>()) == null)
            return;

        // Set all (other) items to unselected (white)
        for (int i = 0; i < _menuTextComponents.Length; i++)
        {
            _menuTextComponents[i].color = Color.white;
            MenuItems[i].transform.localScale = Vector3.one;
        }

        for (int i = 0; i < MenuItems.Length; i++) { 
            if (MenuItems[i] == (GameObject)sender)
                selectedItem = i;
        }

        _menuTextComponents[selectedItem].color = Color.red;
        MenuItems[selectedItem].transform.localScale = Vector3.one * 1.1f;
    }

    void Update () {
        if (!IS_ACTIVE || _newGameClicked || _quitClicked || _continueClicked)
            return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (hit.collider.name == "Sincress" && Input.GetMouseButtonUp(0))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/templates/systems/space-rts-arcade-kit-153179");
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            for (int i = 0; i < _menuTextComponents.Length; i++)
            {
                _menuTextComponents[i].color = Color.white;
                MenuItems[i].transform.localScale = Vector3.one;
            }

            if (selectedItem + 1 < MenuItems.Length)
                selectedItem++;
            _menuTextComponents[selectedItem].GetComponent<Text>().color = Color.red;
            MenuItems[selectedItem].transform.localScale = Vector3.one * 1.1f;

            PlaySound(ScrollSound);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            for (int i = 0; i < _menuTextComponents.Length; i++)
            {
                _menuTextComponents[i].color = Color.white;
                MenuItems[i].transform.localScale = Vector3.one;
            }

            if (selectedItem > 0)
                selectedItem--;
            _menuTextComponents[selectedItem].GetComponent<Text>().color = Color.red;
            MenuItems[selectedItem].transform.localScale = Vector3.one * 1.1f;

            PlaySound(ScrollSound);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (selectedItem)
            {
                case 0: OnStartNewClicked();
                    break;
                case 1: OnContinueGameClicked();
                    break;
                case 2:
                    OnShipViewerClicked();
                    break;
                case 3:
                    OnProfilesClicked();
                    break;
                default:
                    OnQuitClicked();
                    break;
            }
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public void OnStartNewClicked()
    {
        NewGameDialog.SetActive(true);
        _newGameClicked = true;
    }

    public void OnContinueGameClicked()
    {
        if (File.Exists(SaveGame.PERSISTANCE_PATH + "Data/Profiles/" + ProfileMenuController.PLAYER_PROFILE +"/Autosave")) {
            SceneManager.LoadScene("Universe");
        }
        else
        {
            //ContinueGameDialog.SetActive(true);
            //continueClicked = true;
        }
    }

    public void OnShipViewerClicked()
    {
        SceneManager.LoadScene("ShipViewer");
    }

    public void OnProfilesClicked()
    {
        StartCoroutine(MoveToProfileMenu());
    }

    private IEnumerator MoveToProfileMenu()
    {
        IS_ACTIVE = false;
        _timer = 0;
        while (_timer < CAMERA_SHIFT_TIME)
        {
            Camera.main.transform.position = Vector3.Lerp(MainPosition.position, ProfilePosition.position, curve.Evaluate(_timer / CAMERA_SHIFT_TIME));
            _timer += Time.deltaTime;
            yield return null;
        }
    }

    public void OnQuitClicked()
    {
        QuitGameDialog.SetActive(true);
        _quitClicked = true;
    }

    /// <summary>
    /// Callback from dialog menus
    /// </summary>
    public void OnYesClicked()
    {
        if (_newGameClicked)
        {
            SaveGame.CreateNewProfile();
            SceneManager.LoadScene("Universe");
            _newGameClicked = false;
            NewGameDialog.SetActive(false);
        }
        if (_quitClicked)
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }
        if (_continueClicked)
        {
            _continueClicked = false;
        }
    }

    /// <summary>
    /// Callback from dialog menus
    /// </summary>
    public void OnNoClicked()
    {
        if (_newGameClicked)
        {
            _newGameClicked = false;
            NewGameDialog.SetActive(false);
        }
        if (_quitClicked)
        {
            _quitClicked = false;
            QuitGameDialog.SetActive(false);
        }
        if(_continueClicked)
        {
            _continueClicked = false;
        }
    }
}
