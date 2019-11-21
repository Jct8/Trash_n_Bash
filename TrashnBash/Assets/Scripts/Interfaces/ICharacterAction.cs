using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterAction
{
    void TakeDamage(float Dmg, bool isHero);
    IEnumerator Attack();
    void UpdateAnimation();
    IEnumerator DeathAnimation();
}

