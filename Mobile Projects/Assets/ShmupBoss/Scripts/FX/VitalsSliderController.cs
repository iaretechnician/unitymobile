using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Controls Unity UI sliders representing the vitals (health or shield) of an agent (player or enemy).<br></br>
    /// </summary>
    [AddComponentMenu("Shmup Boss/FX/Common/Vitals Slider Controller")]
    [RequireComponent(typeof(AgentCollidable))]
    public class VitalsSliderController : MonoBehaviour
    {
        [Tooltip("The health or shield sliders which will change according to the change in the agent vitals.")]
        [SerializeField]
        private VitalsSlider[] vitalsSliders;

        /// <summary>
        /// The agent whose vitals will control the UI sliders.
        /// </summary>
        private AgentCollidable agent;

        private void Awake()
        {
            agent = GetComponent<AgentCollidable>();

            if (agent == null)
            {
                Debug.Log("VitalsSliderController.cs: Was unable to find agent.");

                return;
            }

            foreach(VitalsSlider vitalsSlider in vitalsSliders)
            {
                vitalsSlider.Initialize(agent);
            }
        }
    }
}