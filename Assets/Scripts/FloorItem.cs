using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloorItem : MonoBehaviour
{
    private bool selected = false; 
    private bool flipNextCycle = false; 
    public static bool globalSelected;
    private LayerMask floorMask;
    private LayerMask furnitureMask;
    private int rotationIndex = 0; // Indexes into the rotations array 
    private float speed = 1f; 

    public Vector3[] rotations = new [] { 
        new Vector3(0,0,0), 
        new Vector3(0,90,0),
        new Vector3(0,180,0), 
        new Vector3(0,270,0)
    };

    public void Start() { 
        floorMask = LayerMask.GetMask("Floor");
        selected = false;
        globalSelected = false;
        furnitureMask = LayerMask.GetMask("Furniture");
    }


    public void Update() { 
        Ray ray;
        RaycastHit hit;
        
        // Detect when object is being clicked on. 
        // Use global to check if other object is being selected to prevent multiple objects from being picked up
        if (Input.GetMouseButtonDown(0)) {
            if (selected) { 
                Debug.Log("DROPPED"); 
                GetComponent<Rigidbody>().mass = 1;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                selected = false; 
                flipNextCycle = true;
                GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
                foreach (GameObject item in items) { 
                    item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y - 2, item.transform.position.z);
                    item.GetComponent<Rigidbody>().useGravity = true;
                    item.GetComponent<Rigidbody>().isKinematic = false;
                }
                return;
            }
            
            ray = CameraButton.currentCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, float.MaxValue, furnitureMask)) {
                Debug.Log("HIT " + hit.transform.name);
                if (hit.transform.name != transform.name) return;
                
                if (!FurnitureItem.globalSelected) { 
                    Debug.Log("PICKED UP " + hit.transform.name); 
                    GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
                    foreach (GameObject item in items) { 
                        item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y + 2, item.transform.position.z);
                        item.GetComponent<Rigidbody>().useGravity = false;
                        item.GetComponent<Rigidbody>().isKinematic = true;
                    }
                    selected = true; 
                    FurnitureItem.globalSelected = true;
                    return;
                }
            }
        }

        // If the object is currently being held by the user:
        // Move towards the user 
        // Ensure the object is in correct rotation and 0 mass (to prevent moving other objects)
        if (selected) { 

            // Handle rotation triggered by right-click
            if (Input.GetMouseButtonDown(1)) {
                if (rotationIndex == rotations.Length - 1) { 
                    rotationIndex = 0; 
                } else { 
                    rotationIndex++;
                }
            }
            ray = CameraButton.currentCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, float.MaxValue, floorMask)) { 
                Vector3 desiredPosition = hit.point;
                Vector3 direction = desiredPosition - transform.position;
                GetComponent<Rigidbody>().velocity = speed * direction;
            }
            
            transform.rotation = Quaternion.Euler(rotations[rotationIndex].x, rotations[rotationIndex].y, rotations[rotationIndex].z);
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            GetComponent<Rigidbody>().mass = 0;
        }

        if (flipNextCycle) { 
            FurnitureItem.globalSelected = false;
            flipNextCycle = false;
        }
    }

}
