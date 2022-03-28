using Polyperfect.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Polyperfect.Crafting.Integration.Demo
{
    public class PlayerCommander:PolyMono
    {
        public override string __Usage => "Sends commands to the player. Typically used via UnityEvents, like those on buttons or Event Triggers. Given events are expected to be PointerEventData.";
        CommandablePlayer player;

        protected void Start()
        {
            player = FindObjectOfType<CommandablePlayer>();
            if (!player)
            {
                Debug.LogError($"There is no player with an attached {nameof(CommandablePlayer)} in the scene, which is necessary for {nameof(PlayerCommander)} scripts.");
                enabled = false;
            }
        }

        public void MoveTo(BaseEventData arg) => player.MoveTo(((PointerEventData)arg).pointerCurrentRaycast.worldPosition);

        public void MoveToOrPlaceAt(BaseEventData data)
        {
            if (data is PointerEventData pointerEventData)
            {
                if (pointerEventData.button == PointerEventData.InputButton.Left)
                    MoveTo(data);
                else if (pointerEventData.button == PointerEventData.InputButton.Right)
                    PlaceAt(data);
                else
                    Debug.LogError("Undefined button");
            }
            else
                Debug.LogError("event was not pointer data");
        }

        public void MoveToObject(Transform trans) => player.MoveTo(trans.position);

        public void MoveToAndInteract(BaseInteractable interactable) => player.MoveToAndInteractWith(interactable);

        public void StopInteracting() => player.StopInteracting();
        public void PlaceAt(BaseEventData arg) => player.PlaceAtPosition(((PointerEventData)arg).pointerCurrentRaycast.worldPosition);
    }
}