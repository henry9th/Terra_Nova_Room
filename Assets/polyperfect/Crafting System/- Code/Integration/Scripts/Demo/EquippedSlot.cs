using Polyperfect.Common;
using Polyperfect.Crafting.Framework;
using UnityEngine;

namespace Polyperfect.Crafting.Integration.Demo
{
    public class EquippedSlot : PolyMono
    {
        public override string __Usage => "Keeps track of which slot is actively equipped.";

        [SerializeField] ItemSlotComponent InitialSlot;

        

        public ItemStackEvent OnEquippedChange = new ItemStackEvent();
        ISlot<Quantity,ItemStack> slot;

        public ISlot<Quantity,ItemStack> Slot
        {
            get => slot;
            set
            {
                if (slot is IChangeable oldChangeable)
                    oldChangeable.Changed -= HandleSlotUpdate;
                if (slot is ItemSlotComponent oldSlotComponent)
                {
                    if (oldSlotComponent.gameObject.TryGetComponent(out EquippableEvents equippable))
                        equippable.SendUnequipped();
                }

                slot = value;
                if (slot is IChangeable newChangeable)
                    newChangeable.Changed += HandleSlotUpdate;
                
                if (slot is ItemSlotComponent newSlotComponent)
                {
                    if (newSlotComponent.gameObject.TryGetComponent(out EquippableEvents equippable))
                        equippable.SendEquipped();
                }
            }
        }

        void HandleSlotUpdate() => OnEquippedChange.Invoke(Slot?.Peek() ?? default);

        void Awake()
        {
            if (InitialSlot)
                Slot = InitialSlot;
        }
    }
}