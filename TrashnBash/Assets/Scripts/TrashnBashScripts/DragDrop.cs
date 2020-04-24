using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour
{
    public bool isDragging = false;
    public GameObject itemToBeDroped;
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if(isDragging)
            {
                Vector2 curPosition = Input.mousePosition;
                transform.position = new Vector2(curPosition.x, curPosition.y);
            }
        }
        if(Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            itemToBeDroped?.GetComponent<IDraggableItem>()?.DropItem();
            Destroy(gameObject);
        }
    }
}
