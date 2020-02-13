using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowAbility : MonoBehaviour, IEnemyAbilities
{
    [SerializeField] private float _maximumRaising = 3.0f;
    [SerializeField] private float _inRangeOfWaypoint = 4.0f;
    private GameObject _crowGO = null;
    private Rigidbody rb = null;

    private bool _isLanding = false;
    private bool _isRising = false;
    public LayerMask groundLayers;

    public void Flying(Transform wayPoint, Order order)
    {
        if(order != Order.Fight || order != Order.Stunned)
        {
            if (Vector3.Distance(wayPoint.position,_crowGO.transform.position) > _inRangeOfWaypoint)
            {
                if (!_isLanding)
                {
                    rb.velocity = Vector3.zero;
                    _crowGO.transform.position = new Vector3(gameObject.transform.position.x, _maximumRaising, gameObject.transform.position.z);
                }
                else
                    _isRising = true;
            }
            else
                _isLanding = true;
        }
        else
        {
            _isLanding = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _crowGO = transform.Find("Model").gameObject;
        rb = transform.Find("Model").gameObject.GetComponent<Rigidbody>();
        _crowGO.transform.position = new Vector3(gameObject.transform.position.x, _maximumRaising, gameObject.transform.position.z);
    }

    void Update()
    {
        if (_isLanding)
        {
            if (_crowGO?.transform.position.y >= 0.5f)
            {
                _crowGO?.transform.Translate(Vector3.down * Time.deltaTime);
            }
        }

        if (_isRising && onGround())
        {
            if (_crowGO.transform.position.y <= _maximumRaising)
            {
                rb.AddForce(0.0f, _maximumRaising, 0.0f, ForceMode.Force);
            }
            else
            {
                _isLanding = false;
                _isRising = false;
            }
        }

    }

    bool onGround()
    {
        return Physics.Raycast(_crowGO.transform.position, -Vector3.up, groundLayers);
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
