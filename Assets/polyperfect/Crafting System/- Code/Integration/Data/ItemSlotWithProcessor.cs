using System;
using Polyperfect.Crafting.Framework;
using UnityEngine;

namespace Polyperfect.Crafting.Integration
{
    public class ItemSlotWithProcessor : ISlot<Quantity, ItemStack>
    {
        readonly Func<ItemStack, ItemStack> _insertPostprocessor;
        Quantity amount;

        RuntimeID id;

        public ItemSlotWithProcessor(Func<ItemStack, ItemStack> insertPostprocessor)
        {
            _insertPostprocessor = insertPostprocessor;
        }

        public ItemStack RemainderIfInserted(ItemStack toInsert)
        {
            if (id.IsDefault() || id.Equals(toInsert.ID))
            {
                var intendedNew = new ItemStack(toInsert.ID, amount + toInsert.Value);
                var actualNew = _insertPostprocessor(intendedNew);
                var difference = actualNew.Value - amount;

                return new ItemStack(toInsert.ID, toInsert.Value - difference);
            }

            return toInsert;
        }

        public ItemStack InsertPossible(ItemStack toInsert)
        {
            if (toInsert.Value <= 0)
                return default;
            var current = Peek();
            if (!current.ID.IsDefault() && !toInsert.ID.Equals(current.ID))
                return toInsert;
            id = toInsert.ID;
            var originalAmount = amount;
            var intendedNew = new ItemStack(id, amount + toInsert.Value);
            var actualNew = _insertPostprocessor(intendedNew);
            id = actualNew.ID;
            amount = actualNew.Value;
            var difference = actualNew.Value - originalAmount;
            return new ItemStack(toInsert.ID, toInsert.Value - difference);
        }

        public ItemStack ExtractAll()
        {
            var ret = new ItemStack(id, amount);
            id = default;
            amount = 0;
            return ret;
        }

        public bool CanExtract()
        {
            return !id.IsDefault();
        }

        public ItemStack Peek()
        {
            return new ItemStack(id, amount);
        }

        public ItemStack ExtractAmount(Quantity arg)
        {
            var ret = new ItemStack(id, Mathf.Min(arg, amount));
            amount -= ret.Value;
            if (amount <= 0)
                id = default;
            return ret;
        }

        public ItemStack Peek(Quantity arg)
        {
            return new ItemStack(id, Mathf.Min(amount, arg));
        }
    }
}