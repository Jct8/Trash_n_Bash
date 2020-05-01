using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class Resource : MonoBehaviour, IDragHandler
{
    public float allowedRangeofResource = 1.2f;
    public float healValue = 10.0f;
    private bool _CanBePickedUp = true;
    private bool _isPlaced = false;
    private Vector3 mousePos;
    private Vector3 ScreenToWorldPoint;

    private void Start()
    {
        VariableLoader variableLoader = ServiceLocator.Get<VariableLoader>();
        if (variableLoader.useGoogleSheets)
        {
            healValue = variableLoader.TrashCanStats["Cooldown"];
        }
    }

    void Update()
    {
        Tower tower = ServiceLocator.Get<LevelManager>().towerInstance.GetComponent<Tower>();

        if(Input.GetMouseButton(0))
        {
            mousePos = Input.mousePosition;
            ScreenToWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10.0f));
            transform.position = ScreenToWorldPoint;
        }

        if (tower)
        {
            if (Vector3.Distance(transform.position, tower.transform.position) <= allowedRangeofResource)
            {
                tower.GetComponent<Tower>().HealTower(healValue);
                Destroy(gameObject);
            }
        }
    }

    public void Pickup(GameObject playerGO)
    {
        float height = transform.position.y;


        GetComponent<NavMeshObstacle>().enabled = false;
        transform.parent = playerGO.transform;
        transform.rotation = playerGO.GetComponent<PlayerController>().GetController().transform.rotation;
        transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 90);

        transform.position = playerGO.transform.position;
        transform.position += playerGO.GetComponent<PlayerController>().GetController().transform.forward;
        transform.position += new Vector3(0.0f, height, 0.0f);
    }

    public bool CanBePickedUp() { return _CanBePickedUp; }

    public void OnDrag(PointerEventData eventData)
    {
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            transform.position = ray.origin + ray.direction * distance;
        }
    }
}