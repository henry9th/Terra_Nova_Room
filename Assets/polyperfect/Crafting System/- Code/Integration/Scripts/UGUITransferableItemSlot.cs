using Polyperfect.Crafting.Integration.UGUI;
using UnityEngine.EventSystems;

namespace Polyperfect.Crafting.Integration
{
    public class UGUITransferableItemSlot : ItemSlotComponent,IPointerDownHandler
    {
        public override string __Usage => $"An item slot that can be transferred to and from using the mouse if a {nameof(UGUIItemTransfer)} is present.";


        public void OnPointerDown(PointerEventData eventData)
        {
            UGUIItemTransfer.Instance.HandleSlotClick(gameObject,eventData);
        }
    }
}