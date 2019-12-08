using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : MonoBehaviour
{
    [SerializeField] private float _Defence  = 10.0f;
    [SerializeField] private float _Health;
    [SerializeField] private float _PercentFromTower = 5.0f;
    [SerializeField] private float repairTime = 3.0f;
    [SerializeField] private float repairRange = 1.0f;

    private bool _CanBePickedUp = true;
    private float _MaxHealth =0.0f;

    public float _barricadeBuildTime = 3.0f;
    public bool inRangeRepair = false;
    public bool isRepairing = false;

    public void PickUp(GameObject playerGO)
    {
        Tower tower = ServiceLocator.Get<GameManager>()._Tower.GetComponent<Tower>() ;
        _Health = tower._health * _PercentFromTower *0.01f;
        _MaxHealth = _Health;
        //tower.TakeDamage(_Health);

        float height = transform.position.y;

        transform.parent = playerGO.transform;
        transform.rotation = playerGO.GetComponent<PlayerController>().GetController().transform.rotation;
        transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 90);

        transform.position = playerGO.transform.position;
        transform.position += playerGO.GetComponent<PlayerController>().GetController().transform.forward;
        transform.position += new Vector3(0.0f, height ,0.0f); 
    }

    public bool CanBePickedUp()
    {
        return _CanBePickedUp;
    }

    public void PlaceBarricade()
    {
        _CanBePickedUp = false;
        transform.parent = null;
    }

    public bool CheckRepairValid(Transform playerTransform)
    {
        if (Vector2.Distance(transform.position, playerTransform.position) < repairRange & isRepairing)
        {
            return true;
        }
        isRepairing = false;
        return false;
    }

    public IEnumerator Repair()
    {
        isRepairing = true;
        yield return new WaitForSeconds(repairTime);
        if (isRepairing)
        {
            _Health = _MaxHealth ;
        }
        isRepairing = false;
    }

}
