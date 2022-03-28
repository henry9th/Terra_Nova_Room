using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Polyperfect.Crafting.Framework;
using ITEM = Polyperfect.Crafting.Integration.ItemStack;

namespace Polyperfect.Crafting.Integration
{
    public class SlottedInventory<SLOT> : IList<ItemStack>, IInsert<IEnumerable<ItemStack>>,IInsert<ItemStack> where SLOT : ISlot<Quantity, ItemStack>
    {
        public List<SLOT> Slots { get; } = new List<SLOT>();

        public IEnumerable<ItemStack> RemainderIfInserted(IEnumerable<ItemStack> toInsert)
        {
            return InventoryOps.RemainderAfterInsertCollection(toInsert, Slots);
        }

        public IEnumerable<ItemStack> InsertPossible(IEnumerable<ItemStack> toInsert)
        {
            return InventoryOps.InsertPossible(toInsert, Slots);
        }

        public IEnumerator<ItemStack> GetEnumerator()
        {
            return Slots.Select(s => s.Peek()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Slots.Select(s => s.Peek()).GetEnumerator();
        }

        public void Add(ItemStack item)
        {
            Slots.First(s => s.CanInsertCompletely(item)).InsertCompletely(item);
        }

        public void Clear()
        {
            foreach (var slot in Slots)
                slot.ExtractAll();
        }

        public bool Contains(ItemStack item)
        {
            return Slots.Select(s => s.Peek()).Contains(item);
        }

        public void CopyTo(ItemStack[] array, int arrayIndex)
        {
            for (var i = 0; i < Slots.Count; i++) array[i + arrayIndex] = Slots[i].Peek();
        }

        public bool Remove(ItemStack item)
        {
            foreach (var slot in Slots)
                if (slot.Peek().Equals(item))
                {
                    slot.ExtractAll();
                    return true;
                }

            return false;
        }

        public int Count => Slots.Count;
        public bool IsReadOnly => false;

        public int IndexOf(ItemStack item)
        {
            for (var i = 0; i < Slots.Count; i++)
                if (Slots[i].Peek().Equals(item))
                    return i;

            return -1;
        }

        public void Insert(int index, ItemStack item)
        {
            while (true)
            {
                if (index >= Slots.Count)
                    throw new ArgumentOutOfRangeException();
                if (Slots[index].Peek().IsDefault())
                {
                    Slots[index].InsertCompletely(item);
                    return;
                }

                var extracted = Slots[index].ExtractAll();
                Slots[index].InsertCompletely(item);
                index += 1;
                item = extracted;
            }
        }

        public void RemoveAt(int index)
        {
            Slots[index].ExtractAll();
        }

        public ItemStack this[int index]
        {
            get => Slots[index].Peek();
            set
            {
                Slots[index].ExtractAll();
                Slots[index].InsertCompletely(value);
            }
        }

        public ItemStack RemainderIfInserted(ItemStack toInsert)
        {
            return InventoryOps.RemainderAfterInsertCollection(toInsert.MakeEnumerable(), Slots).FirstOrDefault();
        }

        public ItemStack InsertPossible(ItemStack toInsert)
        {
            return InventoryOps.InsertPossible(toInsert, Slots);
        }
    }
}