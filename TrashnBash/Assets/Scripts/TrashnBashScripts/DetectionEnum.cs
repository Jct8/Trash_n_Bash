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
    Back
}

public enum DamageType
{
    Normal,
    Poison,
    Ultimate,
    Enemy,
    Skunks
}