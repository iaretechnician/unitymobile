using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SectorIconHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject InfoPanelPrefab;
    public AnimationCurve FadeInCurve, FadeOutCurve;

    private GameObject _infoPanel;
    private SerializableUniverseSector _sectorData;

    public void SetSectorData(SerializableUniverseSector sector)
    {
        _sectorData = sector;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_infoPanel != null)
            Destroy(_infoPanel);

        _infoPanel = Instantiate(InfoPanelPrefab, transform.parent);
        _infoPanel.GetComponent<SectorInfoPanel>().SetValues(_sectorData);
        StartCoroutine(FadeInInfo());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(FadeOutInfo());
    }

    private IEnumerator FadeInInfo()
    {
        Image panelImage = _infoPanel.GetComponent<Image>();

        float t = 0;
        while(t < 0.5f && panelImage != null)
        {
            t += Time.deltaTime;
            panelImage.color = new Color(1, 1, 1, FadeInCurve.Evaluate(t*2f));
            yield return null;
        }
    }

    private IEnumerator FadeOutInfo()
    {
        if(_infoPanel != null && _infoPanel.GetComponent<Image>() != null)
        {
            Image panelImage = _infoPanel.GetComponent<Image>();

            float t = 0;
            while (t < 0.25f && panelImage != null)
            {
                t += Time.deltaTime;
                panelImage.color = new Color(1, 1, 1, FadeOutCurve.Evaluate(t*4f));
                yield return null;
            }
        }

        Destroy(_infoPanel);
    }
}
