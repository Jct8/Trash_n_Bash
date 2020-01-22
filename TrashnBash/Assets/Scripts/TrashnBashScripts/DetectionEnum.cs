using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Detect
{
    None,
    Detected,
    Attack
}

public enum Order
{
    Barricade,
    Tower,
    Fight,
    Back,
    Stunned
}

public enum DamageType
{
    Normal,
    Poison,
    Intimidate,
    Ultimate,
    Enemy,
    Skunks
}