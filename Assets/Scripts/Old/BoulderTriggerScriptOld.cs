using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class BoulderTriggerScriptOld : MonoBehaviour
{
    public HashSet<GameObject> EntityInAreaBefore = new();
    public HashSet<GameObject> EntityInArea = new();
    public HashSet<Vector3> entityInAreaRelativePosBefore = new();
    public HashSet<Vector3> entityInAreaRelativePos = new();
    // public GameObject entityInBoulderAreaBefore;
    // public GameObject entityInBoulderArea;
    // public Vector3 entityInBoulderAreaRelativePosBefore = Vector3.zero;
    // public Vector3 entityInBoulderAreaRelativePos = Vector3.zero;

    public int triggerCount;

    public void OnTickEnd()
    {
        // Get last pos of entity that enter the area, otherwise Zero.
        print(triggerCount);
        if (EntityInArea.Count > 0 && triggerCount <= 3)
        {
            if (EntityInArea.SetEquals(EntityInAreaBefore) &&
                entityInAreaRelativePos.SetEquals(entityInAreaRelativePosBefore))
            {
                triggerCount = 0;
                return;
            }

            EntityInAreaBefore = new HashSet<GameObject>(EntityInArea);
            entityInAreaRelativePosBefore = new HashSet<Vector3>(entityInAreaRelativePos);
            foreach (GameObject entity in EntityInArea)
            {
                entityInAreaRelativePos.Add(entity.transform.position - transform.position);
            }

            foreach (GameObject entity in EntityInArea)
            {
                
            }
            triggerCount++;
        }
        else
        {
            // Out of area (make boulder fall)
            // entityInBoulderAreaRelativePos = Vector3.zero;
            if (triggerCount > 0)
            {
                triggerCount = 3;
            }
        }
    }

    IEnumerator Start()
    {
        yield return new WaitForFixedUpdate();
        EntityInAreaBefore = new HashSet<GameObject>(EntityInArea);
        foreach (GameObject entity in EntityInArea)
        {
            entityInAreaRelativePos.Add(entity.transform.position);
        }
        entityInAreaRelativePosBefore = new HashSet<Vector3>(entityInAreaRelativePos);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == transform.parent || other.name == "AreaOfTrigger")
        {
            return;
        }
        // Debug.Log($"Boulder collided with something {other.name}");
        // entityInBoulderArea = other.gameObject;
        EntityInArea.Add(other.gameObject);
    }   

    private void OnTriggerExit2D(Collider2D other)
    {
        // entityInBoulderArea = null;
        EntityInArea.Remove(other.gameObject);
    }
}
