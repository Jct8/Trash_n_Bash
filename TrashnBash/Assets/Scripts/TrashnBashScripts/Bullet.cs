using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform _target;

    public DamageType damageType = DamageType.Normal;
    public float fireTotalTime = 3.0f;
    public float fireTickTime = 1.0f;
    public Material normalBulletMaterial;
    public Material fireBulletMaterial;

    public ParticleSystem fireParticle;

    public float _speed;
    public float _damage;
    private Action _action;
    public void Initialize(Transform target, float damage, float speed, Action action)
    {
        _target = target;
        _damage = damage;
        _speed = speed;
        _action += action;
    }

    public void OnDisable()
    {
        _action -= _action;
    }

    void Update()
    {
        if(_target == null)
        {
            return;
        }

        Vector3 _direction = Vector3.Normalize(_target.position - transform.position);

        float _distanceOfFrame = _speed * Time.deltaTime;

        transform.Translate(_direction.normalized * _distanceOfFrame, Space.World);
    }

    private void OnTriggerEnter(Collider collision)
    {
        var _damageable = collision.gameObject.GetComponent<ICharacterAction>();
        if(_damageable != null && (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss")) && damageType == DamageType.Normal)
        {
            _damageable.TakeDamage(_damage,false, damageType);
            ResetBullet();
        }
        else if (_damageable != null && (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss")) && damageType == DamageType.Fire) //fire attack
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if(enemy)
                enemy.SetFire(_damage, fireTickTime, fireTotalTime);
            Boss boss = collision.gameObject.GetComponent<Boss>();
            if (boss)
                boss.SetFire(_damage, fireTickTime, fireTotalTime);
            ResetBullet();
        }
    }

    public void SetBulletType(DamageType type)
    {
        damageType = type;
        if (type == DamageType.Fire)
        {
            GetComponent<Renderer>().material.color = Color.red;
            var pSmain = fireParticle.main;
            fireParticle.Simulate(1.0f);
            fireParticle.Play();
            pSmain.simulationSpeed = _speed;
            GetComponent<TrailRenderer>().enabled = true;
        }
        else
            GetComponent<TrailRenderer>().enabled = false;

    }

    void ResetBullet()
    {
        damageType = DamageType.Normal;
        GetComponent<Renderer>().material.color = Color.white;
        fireParticle.Stop();
        GetComponent<TrailRenderer>().enabled = false;
        _action?.Invoke();
    }
}
