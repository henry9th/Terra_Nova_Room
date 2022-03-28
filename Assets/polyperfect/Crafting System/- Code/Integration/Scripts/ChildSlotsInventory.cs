using System;
using System.Collections.Generic;
using System.Linq;
using Polyperfect.Crafting.Framework;
using UnityEngine;
using UnityEngine.Events;
using SLOT = Polyperfect.Crafting.Framework.ISlot<Polyperfect.Crafting.Framework.Quantity,Polyperfect.Crafting.Integration.ItemStack>;

namespace Polyperfect.Crafting.Integration
{
    public class ChildSlotsInventory : BaseItemStackInventory, IChangeable
    {
        [Serializable] class StackAddedEvent:UnityEvent<ItemStack>{}
        public override string __Usage => $"Gets all child slots, like {nameof(ItemSlotComponent)}s. If {nameof(GrabFromSubInventories)} is enabled, it will ignore slots like Discard Slots that do not belong to other inventories.";
        public SlottedInventory<SLOT> inventory { get; private set; }
        [SerializeField] StackAddedEvent OnIndividualItemAdded = new StackAddedEvent();
        [Tooltip("If true, will only get child slots that are already members of other inventories. Useful for avoiding grabbing discard slots for example.")]
        [SerializeField] bool GrabFromSubInventories = false;

        bool initted;
        public override void TryInitSlots()
        {
            if (initted)
                return;
            initted = true;
            inventory = new SlottedInventory<SLOT>();
            if (!GrabFromSubInventories)
            {
                foreach (var slot in GetComponentsInChildren<SLOT>(true))
                {
                    AddSlot(slot);
                }
            }
            else
            {
                foreach (var subInventory in GetComponentsInChildren<BaseItemStackInventory>(true))
                {
                    subInventory.TryInitSlots();
                    foreach (var slot in subInventory.GetSlots())
                    {
                        AddSlot(slot);
                    }
                }
            }
        }

        public event PolyChangeEvent Changed;

        public void SetInventory(SlottedInventory<SLOT> newInventory)
        {
            if (!inventory.IsDefault())
                UnregisterAll();
            inventory = newInventory;
            RegisterAll();
        }

        public void AddSlot(SLOT slot)
        {
            if (inventory.Slots.Contains(slot))
                return;
            inventory.Slots.Add(slot);
            if (slot is IChangeable changeable)
                changeable.Changed += () => Changed?.Invoke();
        }

        public void RemoveSlot(SLOT slot)
        {
            inventory.Slots.Remove(slot);
            if (slot is IChangeable changeable)
                changeable.Changed -= Changed;
        }

        void RegisterAll()
        {
            foreach (var slot in inventory.Slots.Where(s=>s is IChangeable).Cast<IChangeable>()) 
                slot.Changed += Changed;
        }

        void UnregisterAll()
        {
            foreach (var slot in inventory.Slots.Where(s=>s is IChangeable).Cast<IChangeable>()) 
                slot.Changed -= Changed;
        }


        public override IEnumerable<ItemStack> GetItems()
        {
            return inventory.Slots.Select(s => s.Peek());
        }

        public override ICollection<ISlot<Quantity, ItemStack>> GetSlots()
        {
            return inventory.Slots;
        }

        public override ItemStack RemainderIfInserted(ItemStack toInsert)
        {
            return InventoryOps.RemainderAfterInsert(toInsert, inventory.Slots);
        }

        public override ItemStack InsertPossible(ItemStack toInsert)
        {
            var remainder = InventoryOps.InsertPossible(toInsert, inventory.Slots);
            var inserted = new ItemStack(toInsert.ID, toInsert.Value - remainder.Value);
            OnIndividualItemAdded.Invoke(inserted);
            return remainder;
        }

        public override IEnumerable<ItemStack> RemainderIfInserted(IEnumerable<ItemStack> toInsert)
        {
            return InventoryOps.RemainderAfterInsertCollection(toInsert, inventory.Slots);
        }

        public override IEnumerable<ItemStack> InsertPossible(IEnumerable<ItemStack> toInsert)
        {
            return InventoryOps.InsertPossible(toInsert, inventory.Slots);
        }
    }
}