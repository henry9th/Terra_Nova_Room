using System;
using Polyperfect.Crafting.Framework;
using UnityEngine;
using UnityEngine.AI;


namespace Polyperfect.Crafting.Integration.Demo
{
    [DefaultExecutionOrder(10)]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EquippedSlot))]
    public class CommandablePlayer : ItemUserBase
    {
        #region Fields

        public enum ActionType
        {
            None,Move,Place,Interact
        }

        ActionType currentAction;
        public override string __Usage => "Allows various commands such as Move, Gather, and others to be issued.";
        public BaseItemStackInventory TargetInventory;
        public float InteractDistance = 1f;
        public float WalkSpeed = 1.5f;
        public float RunSpeed = 5f;
        public CategoryWithPrefab PlaceableData;
        EquippedSlot _equipped;
        NavMeshAgent _agent;
        BaseInteractable interactTarget;
        bool interacting;
        Vector3 GetDestination()=>_agent.destination;
        #endregion
        new void Start()
        {
            base.Start();
            
            _agent = GetComponent<NavMeshAgent>();
            _equipped = GetComponent<EquippedSlot>();
            if (!TargetInventory)
            {
                Debug.LogError($"No target inventory for {nameof(CommandablePlayer)} on {gameObject.name}.");
                enabled = false;
                return;
            }
        }

        void Update()
        {
            switch (currentAction)
            {
                case ActionType.None:
                    break;
                case ActionType.Move:
                    break;
                case ActionType.Place:
                    if (IsAbleToStartPlacing())
                    {
                        ExecutePlace();
                        StopMoving();
                    }

                    break;
                case ActionType.Interact:
                    if (IsAbleToStartInteracting())
                    {
                        ExecuteInteract();
                        StopMoving();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        void ExecutePlace()
        {
            var pulled = _equipped.Slot?.ExtractAmount(1)??default;
            if (pulled.IsDefault())
                return;
            
            var prefabAccessor = World.GetReadOnlyAccessor<GameObject>(PlaceableData.ID);
            if (!prefabAccessor.TryGetValue(pulled.ID, out var toPlace))
                return;
            
            Instantiate(toPlace, GetDestination(), transform.rotation);
        }

        bool IsAbleToStartPlacing()
        {
            return Vector3.Distance(GetDestination(), transform.position) < InteractDistance;
        }

        bool IsAbleToStartInteracting() => interactTarget && Vector3.Distance(transform.position,interactTarget.transform.position)<InteractDistance;

        public void MoveTo(Vector3 item)
        {
            StopInteracting();
            SetDestination(item);
            currentAction = ActionType.Move;
        }

        Vector3 lastSetDest;
        void SetDestination(Vector3 item)
        {
            var shouldRun = Vector3.Distance(item, lastSetDest) < .5f;
            _agent.speed = shouldRun ? RunSpeed : WalkSpeed;
            lastSetDest = item;
            
            _agent.SetDestination(item);
            _agent.stoppingDistance = .5f;
        }

        public void PlaceAtPosition(Vector3 position)
        {
            StopInteracting();
            SetDestination(position);
            currentAction = ActionType.Place;
        }

        public void StopMoving()
        {
            SetDestination(_agent.nextPosition);
            currentAction = ActionType.None;
        }

        public void StopInteracting()
        {
            currentAction = ActionType.None;
            if (interactTarget&&interacting)
                interactTarget.EndInteract(gameObject);
            interacting = false;
            interactTarget = null;
        }

        public void MoveToAndInteractWith(BaseInteractable target)
        {
            SetDestination(target.transform.position);
            if (interactTarget == target)
                return;
            StopInteracting();   
            _agent.stoppingDistance = InteractDistance;
            interactTarget = target;
            currentAction = ActionType.Interact;
        }

        void ExecuteInteract()
        {
            interacting = true;
            interactTarget.BeginInteract(gameObject);
        }
    }
}