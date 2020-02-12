using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowAbility : MonoBehaviour, IEnemyAbilities
{

    private GameObject _crowGO = null;
    private Rigidbody rb = null;
    [SerializeField] private float _maximumRaising = 3.0f;
    [SerializeField] private float _speedToLanding = 0.02f;
    [SerializeField] private float _distanceToGround = 0.5f;
    private bool _isLanding = false;
    private bool _isRising = false;

    public void Flying(bool fly, Order order)
    {
        if(fly)
        {
            if(onGround() && (order != Order.Fight || order != Order.Stunned) && _isLanding)
            {
                _isLanding = false;
                StartCoroutine("wait");
            }

        }
        else
        {
            _isLanding = true;
            if (_isLanding && !onGround())
            {
                rb.velocity = _speedToLanding * Vector3.up;
            }
        }

        if (_isRising && !(_crowGO.GetComponent<Enemy>().fullHealth < _crowGO.GetComponent<Enemy>()._Health))
        {
            if (_crowGO.transform.position.y != _maximumRaising)
            {
                _crowGO.transform.position = new Vector3(_crowGO.transform.position.x
                , _crowGO.transform.position.y + _speedToLanding,
                _crowGO.transform.position.z);
            }
            else
            {
                _isLanding = false;
                _isRising = false;
                return;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _crowGO = transform.Find("Model").gameObject;
        rb = transform.Find("Model").gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(2.0f);
        _isRising = true;
    }

    bool onGround()
    {
        return Physics.Raycast(_crowGO.transform.position, -Vector3.up, _distanceToGround + 0.1f);
    }

    public void GroupAttack()
    {
        return;
    }

    public void PoisonAOE(GameObject player)
    {
        return;
    }
}
