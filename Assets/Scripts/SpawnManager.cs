using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    public GameObject[] enemyPrefabs;

    [SerializeField]
    public float distanceFromPlayer;

    [SerializeField]
    public float spawnFrequencyInSeconds;

    private int waveIndex;

    [SerializeField]
    private float waveLengthInSeconds;

    [SerializeField]
    private Text waveText;

    private Vector3 playerPosition;
    private Vector3[] spawnLocations;
    private float currentWaveTime;

    private int lastIndex;
    // Start is called before the first frame update
    void Start()
    {
        playerPosition = this.GetComponentInParent<Transform>().position;

        GameObject enemy1 = enemyPrefabs[0];

        spawnLocations = new Vector3[] { 
            new Vector3(playerPosition.x + distanceFromPlayer,
                0,
                playerPosition.z),
            new Vector3(playerPosition.x - distanceFromPlayer,
                0,
                playerPosition.z),
            new Vector3(playerPosition.x,
                0,
                playerPosition.z + distanceFromPlayer),
            new Vector3(playerPosition.x,
                0,
                playerPosition.z - distanceFromPlayer)
        };

        waveIndex = 0;

        Instantiate(enemy1,
            new Vector3(playerPosition.x,
                enemy1.transform.position.y,
                playerPosition.z + distanceFromPlayer),
             Quaternion.identity).transform.LookAt(this.transform);
        lastIndex = 2;
        currentWaveTime = 0.0f;
        FadeInWaveNumber();


    }

    // Update is called once per frame
    void Update()
    {
        currentWaveTime += Time.deltaTime;


    }

    void FadeInWaveNumber()
    {
        waveIndex += 1;
        waveText.text = "Wave " + waveIndex;
        LeanTween.alphaText(waveText.rectTransform, 1f, 3.0f).setOnComplete(() => this.FadeOutWaveNumber());
    }

    void FadeOutWaveNumber()
    {
        currentWaveTime = 0.0f;
        LeanTween.alphaText(waveText.rectTransform, 0f, 3.0f).setOnComplete(() => StartCoroutine("enemySpawner"));
    }

    private GameObject getRandomEnemy()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
    }

    private Vector3 getRandomLocation()
    {
        int currentIndex = Random.Range(0, spawnLocations.Length);
        while (lastIndex == currentIndex)
        {
            currentIndex = Random.Range(0, spawnLocations.Length);
        }
        lastIndex = currentIndex;
        return spawnLocations[currentIndex];
    }

    private IEnumerator enemySpawner()
    {
        Debug.Log("coroutine started");
        while(currentWaveTime <= waveLengthInSeconds)
        {
            yield return new WaitForSeconds(spawnFrequencyInSeconds);
            GameObject enemy = getRandomEnemy();
            Vector3 location = getRandomLocation();
            location = new Vector3(location.x, enemy.transform.position.y, location.z);
            Instantiate(enemy, getRandomLocation(), enemy.transform.rotation).transform.LookAt(this.transform);
        }

        FadeInWaveNumber();

    }
}
