using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barricade : MonoBehaviour
{
    [SerializeField] private float _Defence  = 10.0f;
    [SerializeField] private float _Health;
    [SerializeField] private float _PercentFromTower = 5.0f;
    [SerializeField] private float repairTime = 3.0f;
    [SerializeField] private float repairRange = 1.0f;

    private bool _CanBePickedUp = true;
    private float _MaxHealth =0.0f;
    public Image healthBar;
    public GameObject healthBarGO;
    public float _barricadeBuildTime = 3.0f;
    public bool inRangeRepair = false;
    public bool isRepairing = false;
    public bool isAlive = true;
    public bool isPlaced = false;
    public void PickUp(GameObject playerGO)
    {
        Tower tower = ServiceLocator.Get<LevelManager>().towerInstance.GetComponent<Tower>() ;
        _Health = tower._health * _PercentFromTower *0.01f;
        _MaxHealth = _Health;
        if (_MaxHealth != 0.0f)
            healthBar.fillAmount = _Health / _MaxHealth;
        isAlive = true;
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

    private void Update()
    {
        healthBarGO.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
    }

    public void PlaceBarricade()
    {
        _CanBePickedUp = false;
        transform.parent = null;
        isPlaced = true;
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
            if (_MaxHealth != 0.0f)
                healthBar.fillAmount = _Health / _MaxHealth;
        }
        isRepairing = false;
        isAlive = true;
    }

    public void TakeDamage(float dmg)
    {
        _Health -= dmg;
        if (_MaxHealth != 0.0f)
            healthBar.fillAmount = _Health / _MaxHealth;
        if (_Health <= 0.0f)
        {
            isAlive = false;
        }
    }
}
