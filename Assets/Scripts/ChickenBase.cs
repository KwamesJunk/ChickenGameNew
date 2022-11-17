using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChickenBase : MonoBehaviour
{
    [SerializeField] protected LifeChicken lifeChickenPrefab;
    protected GameObject player;
    protected ChickenSpawner chickenSpawner;
    protected LifeBar lifeBar;

    //[SerializeField] public int number;
    //static int currentNumber = 0;

    //public abstract void Update();
    public abstract void TakeDamage(GameObject killer);


    private void Awake()
    {
        player = GameObject.Find("Player");

        if (!player) print("Player not found");

        //number = currentNumber++;
    }

    public void SetChickenSpawner(ChickenSpawner spawner)
    {
        chickenSpawner = spawner;
    }

    public ChickenSpawner GetChickenSpawner() // temporary kludge
    {
        return chickenSpawner;
    }

    public void SetLife(int newLife)
    {
        //life = newLife;
        GetComponent<HitPoints>().Set(newLife);
    }

    public void SetLifeBar(LifeBar bar)
    {
        lifeBar = bar;
    }
}
