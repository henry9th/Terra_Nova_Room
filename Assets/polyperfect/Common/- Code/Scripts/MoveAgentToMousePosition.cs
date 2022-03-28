using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Polyperfect.Common
{
    [DefaultExecutionOrder(10)]
    [RequireComponent(typeof(NavMeshAgent))]
    public class MoveAgentToMousePosition : PolyMono
    {
        NavMeshAgent _agent;
        Camera _camera;
        readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
        public override string __Usage => "Tells the attached NavMeshAgent to go to the clicked position, ignoring UI";
        public float WalkSpeed=1.4f;
        public float RunSpeed=4f;
        bool DoubleClickToWalk = true;
        
        bool running;
        float lastClickTime;
        const float DoubleClickThreshold = .3f;
        void Start()
        {
            _camera = Camera.main;
            _agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            _agent.speed = running ? RunSpeed : WalkSpeed;
            if (EventSystem.current.currentSelectedGameObject)
                return;
            if (Input.GetMouseButtonDown(0))
            {
                if (Time.time - lastClickTime < DoubleClickThreshold)
                {
                    running = !DoubleClickToWalk;
                    return;
                }
                var pointerEventData = new PointerEventData(EventSystem.current){ position = Input.mousePosition};
                _raycastResults.Clear();
                EventSystem.current.RaycastAll(pointerEventData,_raycastResults);
                foreach (var item in _raycastResults)
                {
                    if (item.module is GraphicRaycaster)
                        continue;
                    SetDestination(item.worldPosition);
                    running = DoubleClickToWalk;
                    lastClickTime = Time.time;
                }

                
            }
        }

        public void SetDestination(Vector3 item)
        {
            _agent.SetDestination(item);
        }
    }
}