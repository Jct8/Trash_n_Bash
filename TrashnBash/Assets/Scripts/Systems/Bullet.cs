using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform _target;

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

    void Update()
    {
        if(_target == null)
        {
            _action?.Invoke();
            return;
        }

        //Vector3 _direction = _target.position - transform.position;
        //float _distanceOfFrame = _speed * Time.deltaTime;
        ////if (_direction.magnitude <= _distanceOfFrame)
        ////{
        ////    Hit();
        ////    return;
        ////}

        //transform.Translate(_direction.normalized * _distanceOfFrame, Space.World);
    }

    //void Hit()
    //{
    //    _action?.Invoke();
    //    _action -= _action;
    //}

    private void OnCollisionEnter(Collision collision)
    {
        var _damageable = collision.gameObject.GetComponent<ICharacterAction>();
        if(_damageable != null && collision.gameObject == gameObject.CompareTag("Enemy"))
        {
            _damageable.TakeDamage(_damage);
            _action?.Invoke();
        }
    }
}
