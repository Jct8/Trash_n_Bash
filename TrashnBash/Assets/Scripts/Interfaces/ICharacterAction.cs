using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterAction
{
    void TakeDamage(float Dmg);
    void Attack(float Dmg);
    void UpdateAnimation();
    IEnumerator DeathAnimation();
}

