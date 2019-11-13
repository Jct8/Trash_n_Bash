using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterAction
{
    void TakeDamage(float Dmg, bool isHero);
    void Attack(float Dmg);
    void UpdateAnimation();
    IEnumerator DeathAnimation();
}

