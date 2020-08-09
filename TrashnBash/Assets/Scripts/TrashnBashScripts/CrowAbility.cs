using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowAbility : MonoBehaviour, IEnemyAbilities
{
    [SerializeField] [Tooltip("Value how much height can Crow raise")] private float _maximumRaising = 3.0f;
    [SerializeField] [Tooltip("Value how much height can Crow land")] private float _minimumHeight = 0.5f;
    [SerializeField] [Tooltip("Value how much range can Crow reach each way-point")] private float _inRangeOfWaypoint = 4.0f;
    public GameObject _crowGO = null;

    private Rigidbody rb = null;
    private Order order = 0;
    private Animator animator;
    private Enemy enemyScript;


    public void Flying(Transform wayPoint)
    {
        order = gameObject.GetComponent<Enemy>()._Order;
        switch (order)
        {
            case Order.Tower:
            {
                if (Vector3.Distance(wayPoint.position, _crowGO.transform.position) > _inRangeOfWaypoint)
                {
                    gameObject.GetComponent<CapsuleCollider>().enabled = false;
                    rb.velocity = Vector3.zero;
                    if (_crowGO.transform.position.y < _maximumRaising)
                        StartCoroutine(Raise());
                    else
                        _crowGO.transform.position = new Vector3(gameObject.transform.position.x, _maximumRaising, gameObject.transform.position.z);
                }
                else
                {
                    StartCoroutine(Land());
                }
                break;
            }
            case Order.Barricade:
            case Order.Stunned:
            case Order.Fight:
            {
                if (_crowGO.transform.position.y > _minimumHeight)
                    StartCoroutine(Land());
                break;
            }
            case Order.Back:
            {

                if (_crowGO.transform.position.y < _maximumRaising)
                    StartCoroutine(Raise());
                break;
            }
        }

    }

    void Start()
    {
        _crowGO = transform.Find("Model").gameObject;
        rb = transform.Find("Model").gameObject.GetComponent<Rigidbody>();
        _crowGO.transform.position = new Vector3(gameObject.transform.position.x, _maximumRaising, gameObject.transform.position.z);
        animator = GetComponent<Animator>();
        enemyScript = GetComponent<Enemy>();

    }

    void Update()
    {
        if (gameObject.GetComponent<Enemy>().IsDead)
            StartCoroutine(Land());
    }

    private IEnumerator Land()
    {
        gameObject.GetComponent<CapsuleCollider>().enabled = true;
        while (_crowGO?.transform.position.y > _minimumHeight)
        {
            _crowGO?.transform.Translate(Vector3.down * Time.deltaTime);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator Raise()
    {
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        while (_crowGO?.transform.position.y < _maximumRaising)
        {
            _crowGO?.transform.Translate(Vector3.up * Time.deltaTime);
            yield return new WaitForSeconds(0.5f);
        }
        if (animator && !enemyScript.IsDead)
            animator.SetBool("Attacking", false);
    }

    public void GroupAttack()
    {
        return;
    }

    public void PoisonAOE()
    {
        return;
    }

    public void PlayDead()
    {
        return;
    }
}
