using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawner : MonoBehaviour
{
    [SerializeField] GameObject chickenPrefab;
    [SerializeField] GameObject bigChickenPrefab;
    [SerializeField] GameObject giantChickenPrefab;
    [SerializeField] GameObject meteorChickenPrefab;
    [SerializeField] UnityEngine.UI.Text enemyCount;
    [SerializeField] UnityEngine.UI.Text killCountDisplay;
    [SerializeField] UnityEngine.UI.Text waveDisplay;
    [SerializeField] PlayerCamera playerCamera;
    GameObject player;
    float spawnDelay = 1.0f;
    float timeKeeper = 0.0f;
    float radius = 5.0f;
    const float HALF_PI = 1.5707963267948966192313216916398f;
    List<GameObject> chickenList;
    int killCount = 0;
    int wave = 0;
    int waveKillCount = 0;
    const int DEFAULT_NUM_CHICKENS = 5; //20;
    const float METEOR_CHICKEN_SPAWN_DELAY = 0.5f;
    float meteorChicken_tk = 0.0f;

    bool invalidWave = false;

    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        chickenList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (timeKeeper >= spawnDelay) {
            
        //    timeKeeper = 0.0f;
        //    RandomPointOnCircle(radius, out float x, out float z); // initializing inline

        //    GameObject chicken = null;
        //    if (chickenList.Count < 20) {
        //        chicken = Instantiate(chickenPrefab, player.transform.position + new Vector3(x, 10.0f, z), Quaternion.identity);
        //        //chicken.GetComponent<ChickenController2>().SetChickenSpawner(this);
        //        SetChickenSpawner(chicken);
        //        chickenList.Add(chicken);
        //    }

        //    if (killCount >= 1 && !bigAwake) {
        //        chicken = Instantiate(bigChickenPrefab, player.transform.position + new Vector3(z, 10.0f, x), Quaternion.identity);
        //        bigAwake = true;
        //        //chicken.GetComponent<ChickenController2>().SetChickenSpawner(this);
        //        SetChickenSpawner(chicken);
        //        chickenList.Add(chicken);
        //    }
            
        //}

        if (timeKeeper >= spawnDelay) {

            timeKeeper = 0.0f;

            switch(wave) {
                case 0:
                    if (waveKillCount + chickenList.Count < DEFAULT_NUM_CHICKENS) {
                        if (chickenList.Count < 10) {
                            SpawnChickenOfDeath();
                        }
                    }

                    // Transition to next wave
                    if (waveKillCount >= DEFAULT_NUM_CHICKENS) { // killcount is 20
                        NextWave();     
                    }
                    break;
                case 1: // Spawn Big Chicken
                    SpawnBigChicken();                   
                    NextWave();
                    break;
                case 2:
                    if (chickenList.Count == 0) { // Big Chicken is dead
                        
                        playerCamera.Reset();// zoom in here
                        NextWave();
                    }
                    break;
                case 3: // Chickens
                    if (waveKillCount + chickenList.Count < DEFAULT_NUM_CHICKENS) {
                        if (chickenList.Count < 10) {
                            SpawnChickenOfDeath();
                        }
                    }

                    // Transition to next wave
                    if (waveKillCount >= DEFAULT_NUM_CHICKENS) { // killcount is 20 so spawn Big Chicken
                        NextWave();
                    }
                    break;
                case 4: // Meteor Chickens
                    if (waveKillCount < DEFAULT_NUM_CHICKENS) {
                        SpawnMeteorChicken();
                    }
                    else {
                        NextWave();
                    }
                    break;
                case 5:
                    if (waveKillCount < DEFAULT_NUM_CHICKENS / 2) {
                        SpawnChickenOfDeath();
                    }
                    else {
                        NextWave();
                    }
                    break;
                case 6:
                    if (waveKillCount < DEFAULT_NUM_CHICKENS) {
                        SpawnMeteorChicken();

                        if (waveKillCount + chickenList.Count < DEFAULT_NUM_CHICKENS) {
                            SpawnChickenOfDeath();
                        }
                    }
                    else {
                        waveKillCount = 0;
                    }
                    break;
                default:
                    if (!invalidWave) {
                        print("Invalid wave.");
                        invalidWave = true;
                    }
                    break;
            }
        }

        CollectGarbage();
        enemyCount.text = "Enemy Count: " + chickenList.Count;
        killCountDisplay.text = "Kill Count: " + killCount;

        timeKeeper += Time.deltaTime;

        //MeteorTester();
    }

    float  RandomPlusOrMinusOne()
    {
        if (Random.Range(0, 2) == 0) return -1.0f;

        return 1.0f;
    }

    void CollectGarbage()
    {
        chickenList.RemoveAll(IsDestroyed);
    }

    private static bool IsDestroyed(GameObject obj)
    {
        return (obj == null);
    }

    public void IncrementKillCount()
    {
        ++killCount;
        ++waveKillCount;

        //print("IncrementKillCount called.");
    }

    void MeteorTester()
    {
        //-9, -35
        //xrot:135-225
        //zrot:

        if (Random.Range(0, 30) == 0) {
            Vector3 pos = new Vector3(Random.Range(-24.0f, 6.0f), 20.0f, Random.Range(-50.0f, -20.0f));
            GameObject m = Instantiate(meteorChickenPrefab, pos, Quaternion.Euler(Random.Range(135.0f, 225.0f), Random.Range(0.0f, 360.0f), 0));
            //Destroy(m, 2.0f);
        }
    }

    void SpawnMeteorChicken()
    {
        if (Time.time - meteorChicken_tk > METEOR_CHICKEN_SPAWN_DELAY) {
            meteorChicken_tk = Time.time;

            Vector3 pos = new Vector3(Random.Range(-5.0f, 5.0f) + player.transform.position.x, 20.0f, Random.Range(-5.0f, 5.0f) + player.transform.position.z);
            GameObject m = Instantiate(meteorChickenPrefab, pos, Quaternion.Euler(Random.Range(135.0f, 225.0f), Random.Range(0.0f, 360.0f), 0));

            ++waveKillCount;
        }
    }

    void SetChickenSpawnerToThis(GameObject obj)
    {
        if (obj.GetComponent<ChickenBase>()) {
            obj.GetComponent<ChickenBase>().SetChickenSpawner(this);
        }
        else {
            print("SetChickenSpawner error on "+obj.name+"!");
        }


    }

    void RandomPointOnCircle(float rad, out float x, out float z)
    {
        float angle = Random.Range(0, HALF_PI);
        x = rad * Mathf.Cos(angle) * RandomPlusOrMinusOne(); // randomly choose + or -
        z = rad * Mathf.Sin(angle) * RandomPlusOrMinusOne(); // only calculate first quarter-circle
    }

    void SpawnBigChicken()
    {
        RandomPointOnCircle(radius, out float x, out float z); // initializing inline
        GameObject bigChicken = Instantiate(bigChickenPrefab, player.transform.position + new Vector3(z, 10.0f, x), Quaternion.identity);
        SetChickenSpawnerToThis(bigChicken);
        chickenList.Add(bigChicken);
    }

    void SpawnChickenOfDeath()
    {
        RandomPointOnCircle(radius, out float x, out float z); // initializing inline
        GameObject chicken = Instantiate(chickenPrefab, player.transform.position + new Vector3(x, 10.0f, z), Quaternion.identity);
        SetChickenSpawnerToThis(chicken);
        chickenList.Add(chicken);
    }

    void NextWave()
    {
        ++wave;
        waveKillCount = 0;
        waveDisplay.text = "Wave: " + wave;
    }
}
