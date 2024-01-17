using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    [SerializeField]
    GameObject tower;

    // Update is called once per frame
    void Update()
    {
        if(IsMouseOverObject())
        {
            GameObject towerObject = Instantiate(tower);
            towerObject.transform.position = transform.position;
            Destroy(gameObject);
        }
    }

    bool IsMouseOverObject()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
    }
}
