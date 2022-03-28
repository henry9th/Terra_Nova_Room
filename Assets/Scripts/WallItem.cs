using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallItem : MonoBehaviour
{
    private bool selected = false; 
    private bool flipNextCycle = false;
    private LayerMask rightWallMask;
    private LayerMask leftWallMask;
    private LayerMask wallItemMask;
    private int rotationIndex = 0; // Indexes into the rotations array 
    private float speed = 2f; 

    public Vector3[] rotations = new [] { 
        new Vector3(0,0,0), 
        new Vector3(0,270,0),
    };

    public void Start() { 
        rightWallMask = LayerMask.GetMask("Right Wall");
        leftWallMask = LayerMask.GetMask("Left Wall");
        wallItemMask = LayerMask.GetMask("Wall Item");

        selected = false;
    }


    public void Update() { 
        Ray ray;
        RaycastHit hit;
        
        // Detect when object is being clicked on. 
        // Use global to check if other object is being selected to prevent multiple objects from being picked up
        if (Input.GetMouseButtonDown(0)) {

            // Determine which wall the wall item was placed on, use raycast to determine where to stick the wall item to 
            if (selected) { 
                ray = CameraButton.currentCamera.ScreenPointToRay(Input.mousePosition);
                // If Left wall 
                bool rayHit = false;
                if (rayHit = Physics.Raycast(ray, out hit, float.MaxValue, leftWallMask)) { 
                    ray = new Ray(transform.position, transform.forward * -1);
                    if (rayHit = Physics.Raycast(ray, out hit, float.MaxValue, leftWallMask)) {
                        transform.position = hit.point;
                    }
                // If Right wall  
                } else if (rayHit = Physics.Raycast(ray, out hit, float.MaxValue, rightWallMask)) { 
                    ray = new Ray(transform.position, transform.forward * -1); // invert foward 
                    if (rayHit = Physics.Raycast(ray, out hit, float.MaxValue, rightWallMask)) {
                        transform.position = hit.point;
                    }
                }
                   
                if (rayHit) { 
                    GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    GetComponent<Rigidbody>().isKinematic = true;
                    selected = false; 
                    flipNextCycle = true;
                    return;
                }
            }
            
            ray = CameraButton.currentCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, float.MaxValue, wallItemMask)) {
                if (hit.transform.name != transform.name) return;
                
                if (!FurnitureItem.globalSelected) { 
                    GetComponent<Rigidbody>().isKinematic = false;
                    selected = true; 
                    FurnitureItem.globalSelected  = true;
                    return;
                }
            }
        }

        // If the object is currently being held by the user:
        // Move towards the user 
        // Ensure the object is in correct rotation and 0 mass (to prevent moving other objects)
        if (selected) { 

            // Handle rotation triggered by right-click (CURRENTLY DONT ALLOW FOR WALL ITEMS)
            // if (Input.GetMouseButtonDown(1)) {
            //     rotationIndex = (rotationIndex == 0) ? 1: 0;
            // }

            ray = CameraButton.currentCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, float.MaxValue, leftWallMask)) { 
                rotationIndex = 1;
                Vector3 desiredPosition = hit.point;
                Vector3 direction = desiredPosition - transform.position;
                GetComponent<Rigidbody>().velocity = speed * direction;
            } else if (Physics.Raycast(ray, out hit, float.MaxValue, rightWallMask)) { 
                rotationIndex = 0;
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
