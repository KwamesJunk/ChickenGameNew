using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HitPoints : MonoBehaviour
{
    int hp = 1;
    public event Action onChange;
    public event Action onZeroOrLess;

    public void Awake()
    {
        onChange += Dummy;
        onZeroOrLess += Dummy;
    }

    public int Get()
    {
        return hp;
    }

    public void Set(int p_hp)
    {
        hp = p_hp;
        onChange();
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
