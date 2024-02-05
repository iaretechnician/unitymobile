using UnityEngine;

/// <summary>
/// Class containing prefab references
/// </summary>
[CreateAssetMenu(menuName = "DataHolders/UIElements")]
public class UIElements : SingletonScriptableObject<UIElements> {

    public GameObject InformationText;

    [Header("Menus")]
    public GameObject ScrollMenu;
    public GameObject ScrollText;
    public GameObject SimpleMenu;
    public GameObject TargetMenu;

    [Header("Dialogs")]
    public GameObject SliderDialog;
    public GameObject InputDialog;
    public GameObject ConfirmDialog;

    [Header("Elements")]
    public GameObject ClickableText;
    public GameObject ClickableTextChoice;
    public GameObject ClickableImageText;
    public GameObject TextPanel;
    public GameObject TwoTextPanel;

}
