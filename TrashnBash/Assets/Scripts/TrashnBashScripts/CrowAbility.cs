using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowAbility : MonoBehaviour, IEnemyAbilities
{
    [SerializeField] private float _maximumRaising = 3.0f;
    [SerializeField] private float _minimumHeight = 0.5f;
    [SerializeField] private float _inRangeOfWaypoint = 4.0f;
    public GameObject _crowGO = null;
    private Rigidbody rb = null;
    private Order order = 0;
    public bool _isLanding = false;
    private bool _isRising = false;
    public LayerMask groundLayers;

    public void Flying(Transform wayPoint)
    {
        order = gameObject.GetComponent<Enemy>()._Order;
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

    void Start()
    {
        _crowGO = transform.Find("Model").gameObject;
        rb = transform.Find("Model").gameObject.GetComponent<Rigidbody>();
        _crowGO.transform.position = new Vector3(gameObject.transform.position.x, _maximumRaising, gameObject.transform.position.z);
    }

    void Update()
    {
        order = gameObject.GetComponent<Enemy>()._Order;
        if(order != Order.Fight || order != Order.Stunned)
        {
            if (_isLanding) // When Crow reaches to the object: Tower and barricade
            {
                if (_crowGO?.transform.position.y >= _minimumHeight)
                {
                    _crowGO?.transform.Translate(Vector3.down * Time.deltaTime);
                }
            }

            if (_isRising) // When Crow succeed to break the tower or the barricade
            {
                if (_crowGO.transform.position.y < _maximumRaising)
                {
                    _crowGO.transform.Translate(new Vector3(0.0f, _maximumRaising, 0.0f) * Time.deltaTime);
                }
                else
                {
                    _isLanding = false;
                    _isRising = false;
                }
                if (_crowGO.transform.position.y > _maximumRaising) // if Crow's height exceeds over maximum height, matches height correctly
                    _crowGO.transform.Translate(new Vector3(0.0f, _maximumRaising, 0.0f) * Time.deltaTime);

            }
        }
        else // When Crow takes damage by the player
        {
            _isLanding = false;
            _isRising = false;
            if (_crowGO?.transform.position.y >= _minimumHeight)
            {
                _crowGO?.transform.Translate(Vector3.down * Time.deltaTime);
            }
        }
    }

    public void GroupAttack()
    {
        return;
    }

    public void PoisonAOE(GameObject player)
    {
        return;
    }

    public void PlayDead(GameObject player)
    {
        return;
    }
}
