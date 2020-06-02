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
            //_action?.Invoke();
            return;
        }

        Vector3 _direction = Vector3.Normalize(_target.position - transform.position);
        //transform.position = _direction * _speed * Time.deltaTime;
        float _distanceOfFrame = _speed * Time.deltaTime;
        //if (_direction.magnitude <= _distanceOfFrame)
        //{
        //    Hit();
        //    return;
        //}

        transform.Translate(_direction.normalized * _distanceOfFrame, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var _damageable = collision.gameObject.GetComponent<ICharacterAction>();
        if(_damageable != null && collision.gameObject.CompareTag("Enemy") && damageType == DamageType.Normal)
        {
            _damageable.TakeDamage(_damage,false, damageType);
            ResetBullet();
        }
        else if (_damageable != null && collision.gameObject.CompareTag("Enemy") && damageType == DamageType.Poison) //fire attack
        {
            collision.gameObject.GetComponent<Enemy>().SetPoison(_damage, fireTickTime, fireTotalTime);
            ResetBullet();
        }
    }

    public void SetBulletType(DamageType type)
    {
        damageType = type;
        if (type == DamageType.Poison)
        {
            GetComponent<Renderer>().material.color = Color.red;
            var pSmain = fireParticle.main;
            pSmain.simulationSpeed = 10.0f;
            fireParticle.Play();
        }
    }

    void ResetBullet()
    {
        damageType = DamageType.Normal;
        GetComponent<Renderer>().material.color = Color.white;
        fireParticle.Stop();
        _action?.Invoke();
    }
}
