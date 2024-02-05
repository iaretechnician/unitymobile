using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;

/// <summary>
/// Handles input on the profile selection menu of the Main Menu scene.
/// </summary>
public class ProfileMenuController : MonoBehaviour {

    public static string PLAYER_PROFILE;

    public GameObject[] MenuItems;
    private Text[] _menuTextComponents;

    public GameObject DialogCanvas;
    public UIElements UIElements;

    public AudioClip ScrollSound, ConfirmSound;
    private AudioSource _audioSource;

    private int _selectedItem = 0;
    private int _numberOfProfiles = 0;

    // Camera animation controls
    public AnimationCurve curve;
    public Transform ProfilePosition, MainPosition;
    private float _timer;
    private static float CAMERA_SHIFT_TIME = 2f;

    private void Awake()
    {
        // Extract text components
        _menuTextComponents = new Text[MenuItems.Length];
        for (int j = 0; j < MenuItems.Length; j++)
        {
            _menuTextComponents[j] = MenuItems[j].GetComponentInChildren<Text>();
        }

        // Load existing profiles
        if (!Directory.Exists(SaveGame.PERSISTANCE_PATH + "Data/Profiles"))
        {
            Directory.CreateDirectory(SaveGame.PERSISTANCE_PATH + "Data/Profiles");
        }

        var profiles = Directory.GetDirectories(SaveGame.PERSISTANCE_PATH + "Data/Profiles");
        _numberOfProfiles = profiles.Length;
        int i;
        for (i = 0; i < _numberOfProfiles; i++)
        {
            MenuItems[i].SetActive(true);
            _menuTextComponents[i].text = new DirectoryInfo(profiles[i]).Name;
        }
        MenuItems[i].SetActive(true);
        _menuTextComponents[i].text = "Create new";

        EventManager.PointerEntry += OnPointerEntry;

        _menuTextComponents[_selectedItem].color = Color.red;
        MenuItems[_selectedItem].transform.localScale = Vector3.one * 1.1f;
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

        for (int i = 0; i < MenuItems.Length; i++)
        {
            if (MenuItems[i] == (GameObject)sender)
                _selectedItem = i;
        }

        _menuTextComponents[_selectedItem].color = Color.red;
        MenuItems[_selectedItem].transform.localScale = Vector3.one * 1.1f;
    }

    void Update()
    {
        if (MainMenuController.IS_ACTIVE)
            return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            for (int i = 0; i < _menuTextComponents.Length; i++)
            {
                _menuTextComponents[i].color = Color.white;
                MenuItems[i].transform.localScale = Vector3.one;
            }

            if (_selectedItem + 1 < MenuItems.Length)
                _selectedItem++;
            _menuTextComponents[_selectedItem].GetComponent<Text>().color = Color.red;
            MenuItems[_selectedItem].transform.localScale = Vector3.one * 1.1f;

            PlaySound(ScrollSound);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            for (int i = 0; i < _menuTextComponents.Length; i++)
            {
                _menuTextComponents[i].color = Color.white;
                MenuItems[i].transform.localScale = Vector3.one;
            }

            if (_selectedItem > 0)
                _selectedItem--;
            _menuTextComponents[_selectedItem].GetComponent<Text>().color = Color.red;
            MenuItems[_selectedItem].transform.localScale = Vector3.one * 1.1f;

            PlaySound(ScrollSound);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnItemClicked(_selectedItem);
        }

    }

    public void OnItemClicked(int selectedItem)
    {
        if (selectedItem < _numberOfProfiles) {
            PLAYER_PROFILE = MenuItems[selectedItem].GetComponentInChildren<Text>().text;
            StartCoroutine(MoveToMainMenu());
        }
        else
        {
            CreateNewProfile();
        }
    }

    private void CreateNewProfile()
    {
        // Open Confirm Dialog
        GameObject SubMenu = GameObject.Instantiate(UIElements.InputDialog, DialogCanvas.transform);
        // Reposition submenu
        RectTransform rt = SubMenu.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, 0);

        PopupInputMenuController confirmSaleMenu = SubMenu.GetComponent<PopupInputMenuController>();
        confirmSaleMenu.TextInput.contentType = InputField.ContentType.Alphanumeric;
        confirmSaleMenu.HeaderText.text = "Enter profile name";

        confirmSaleMenu.AcceptButton.onClick.AddListener(() => {
            MenuItems[_numberOfProfiles].SetActive(true);
            _menuTextComponents[_numberOfProfiles].text = confirmSaleMenu.TextInput.text;
            _numberOfProfiles++;
            if (_numberOfProfiles < MenuItems.Length)
                MenuItems[_numberOfProfiles].SetActive(true);
            GameObject.Destroy(confirmSaleMenu.gameObject);
        });
        confirmSaleMenu.CancelButton.onClick.AddListener(() => {
            GameObject.Destroy(confirmSaleMenu.gameObject);
        });
    }

    private IEnumerator MoveToMainMenu()
    {
        MainMenuController.IS_ACTIVE = true;
        _timer = 0;
        while (_timer < CAMERA_SHIFT_TIME) { 
            Camera.main.transform.position = Vector3.Lerp(ProfilePosition.position, MainPosition.position, curve.Evaluate(_timer / CAMERA_SHIFT_TIME));
            _timer += Time.deltaTime;
            yield return null;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = clip;
        _audioSource.Play();
    }

}
