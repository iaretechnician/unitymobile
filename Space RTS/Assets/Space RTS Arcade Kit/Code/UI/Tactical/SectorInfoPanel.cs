using UnityEngine;
using UnityEngine.UI;

public class SectorInfoPanel : MonoBehaviour
{
    private Vector3 OFFSET = new Vector3(Screen.width / 2 + 30f, Screen.height / 2 + 30f);

    public Text SectorName;
    public Text IsTaken;
    public Text Difficulty;

    private RectTransform _rect;

    public void SetValues(SerializableUniverseSector sectorData)
    {
        SectorName.text = sectorData.Name;
        IsTaken.text = Player.SectorsTaken.Contains(sectorData.Name) ? "Sector already taken" : "Sector not occupied";
        Difficulty.text = "Difficulty: "+sectorData.Difficulty;
    }

    private void Awake()
    {
        if (_rect == null)
        {
            _rect = GetComponent<RectTransform>();
        }

        _rect.localPosition = Input.mousePosition - OFFSET;
    }

    private void Update()
    {
        Awake();
    }
}