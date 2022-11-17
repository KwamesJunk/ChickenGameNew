using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HitPoints : MonoBehaviour
{
    [SerializeField]int hp = 1;
    [SerializeField]int maxHp = 1;
    public event Action onChange;
    public event Action onZeroOrLess;
    bool isInitialized = false;

    public void Awake()
    {
        onChange += Dummy;
        onZeroOrLess += Dummy;
        isInitialized = true;
    }

    public void Start()
    {
        hp = maxHp;
    }

    public int Get()
    {
        return hp;
    }

    public int GetMax()
    {
        return maxHp;
    }

    public void Set(int p_hp)
    {
        if (isInitialized) {
            hp = p_hp;

            //if (onChange == null)
            //    print("Action Delegate is null (onChange)");
            //else
                onChange();
        }
    }

    public void SetMax(int p_maxHp)
    {
        maxHp = p_maxHp;
    }

    public void SetToMax()
    {
        if (isInitialized) {
            Set(maxHp);
            onChange();
        }
    }

    public void Increment()
    {
        ++hp;
        onChange();
    }

    public void Increment(int amount)
    {
        hp += amount;
        onChange();
    }

    public void Decrement()
    {
        --hp;
        onChange();

        //print("Hp: " + hp);
        if (hp <= 0) {
            onZeroOrLess();
        }
    }

    public void Decrement(int amount) // this isn't being called, but it's being called
    {
        hp -= amount;
        onChange();

        //print("Hp: " + hp);
        //print("Decrement by " + amount);
        if (hp <= 0) {
            onZeroOrLess();
            //print("Zero or less");
        }
    }

    void Dummy()
    {
    }
}
