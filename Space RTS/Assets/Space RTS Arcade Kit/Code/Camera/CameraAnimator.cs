using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CameraAnimator : MonoBehaviour
{
    public Transform Position1, Position2;
    public float AnimationTime = 1f;
    public AnimationCurve CameraAnimationCurve;

    private void Start()
    {
        Camera.main.transform.position = Position1.position;
        StartCoroutine(AnimateCamera(Position2.position, null));
    }

    public void SwitchToSector(string sectorName)
    {
        StartCoroutine(AnimateCamera(Position1.position, sectorName));
    }

    private IEnumerator AnimateCamera(Vector3 endposition, string sectorName)
    {
        float t = 0;
        Vector3 startPosition = Camera.main.transform.position;

        while (t < AnimationTime)
        {
            t += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(startPosition, endposition, CameraAnimationCurve.Evaluate(t / AnimationTime));
            Camera.main.transform.rotation = Quaternion.Euler(Vector3.Lerp(Position1.rotation.eulerAngles, Position2.rotation.eulerAngles, CameraAnimationCurve.Evaluate(t / AnimationTime)));
            yield return null;

        }

        Camera.main.transform.position = endposition;
        if (sectorName != null)
        {
            SectorNavigation.ChangeSector(sectorName, false);
            SceneManager.LoadSceneAsync("EmptyFlight", LoadSceneMode.Single);
        }
    }
}
