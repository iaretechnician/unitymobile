using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour {

    // TWo sources are needed for crossfading
    public AudioSource AmbientSrc, BattleSrc;
    public AudioClip Ambient, Battle;
    [Tooltip("How often the Music Controller will check the distance between the player ship and" +
        "the closest enemy ship to change music")]
    public float CheckInterval = 2f;
    [Tooltip("Distance below which the Battle music replaces Ambient music")]
    public int BattleDistanceThreshold = 1000;
    public float CrossfadeDuration = 3f;

    private float checkTimer = 0f;
    private bool inBattleMode = false;

    private void Start()
    {
        checkTimer = CheckInterval;
        // All clear, chill out mode
        StartCoroutine(SwitchToAmbientMode(Ambient));
    }

    private void Update()
    {
        checkTimer -= Time.deltaTime;
        if(checkTimer < 0)
        {
            checkTimer = CheckInterval;

            if (Ship.PlayerShip == null)
                return;
            // Check distance to closest enemy ship
            Vector3 playerPosition = Ship.PlayerShip.transform.position;
            var enemies = SectorNavigation.Instance.GetClosestEnemyShip(Ship.PlayerShip.transform, 5000);

            if (enemies.Count == 0 && inBattleMode)
            {
                // No enemy in range.
                StartCoroutine(SwitchToAmbientMode(Ambient));
            }
            if(enemies.Count > 0 && inBattleMode)
            {
                if(Vector3.Distance(playerPosition, enemies[0].transform.position) > BattleDistanceThreshold) { 
                    // All clear, chill out mode
                    StartCoroutine(SwitchToAmbientMode(Ambient));
                }
            }
            if(enemies.Count > 0 && !inBattleMode)
            {
                if (Vector3.Distance(playerPosition, enemies[0].transform.position) < BattleDistanceThreshold) { 
                    // Danger close, start war drums
                    StartCoroutine(SwitchToBattleMode(Battle));
                }
            }
        }
    }

    private IEnumerator SwitchToAmbientMode(AudioClip ambientClip)
    {
        inBattleMode = false;

        AmbientSrc.clip = ambientClip;
        AmbientSrc.Play();
        AmbientSrc.volume = 0;

        while(AmbientSrc.volume < 1.0)
        {
            AmbientSrc.volume += Time.deltaTime / CrossfadeDuration;
            if (BattleSrc.isPlaying && BattleSrc.volume >= 0)
                BattleSrc.volume -= Time.deltaTime / CrossfadeDuration;

            yield return null;
        }

        BattleSrc.Stop();
        AmbientSrc.volume = 1.0f;

        // Crossfade
        yield return null;
    }

    private IEnumerator SwitchToBattleMode(AudioClip battleClip)
    {
        inBattleMode = true;

        BattleSrc.clip = battleClip;
        BattleSrc.Play();
        BattleSrc.volume = 0;

        while (BattleSrc.volume < 1.0)
        {
            BattleSrc.volume += Time.deltaTime / CrossfadeDuration;
            if (AmbientSrc.isPlaying && BattleSrc.volume >= 0)
                AmbientSrc.volume -= Time.deltaTime/CrossfadeDuration;

            yield return null;
        }

        AmbientSrc.Stop();
        BattleSrc.volume = 1.0f;

        // Crossfade
        yield return null;
    }


}
