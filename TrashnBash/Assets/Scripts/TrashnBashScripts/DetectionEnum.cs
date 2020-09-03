using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Order
{
    Barricade,
    Tower,
    Fight,
    Back,
    Stunned
}

public enum Boss_Order
{
    Move,
    Fight,
    StunAttack,
    PoisonAttack,
    Back,
    Waiting,
    Stunned
}

public enum DamageType
{
    Normal,
    Poison,
    Intimidate,
    Ultimate,
    Enemy,
    Skunks,
    Loading,
    Fire
}