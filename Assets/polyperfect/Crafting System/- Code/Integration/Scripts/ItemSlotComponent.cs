using Polyperfect.Crafting.Framework;
using UnityEngine;
using UnityEngine.Events;

namespace Polyperfect.Crafting.Integration
{
    public class ItemSlotComponent : ItemUserBase, IChangeable, ISlot<Quantity, ItemStack>
    {
        public bool IsReadonly;
        [SerializeField] Quantity MaximumCapacity = 64;
        [SerializeField] ObjectItemStack InitialObject;
        [SerializeField] BaseCategoryObject MemberRequirement;
        [SerializeField] CategoryWithInt CapacitySource;
        [SerializeField] UnityEvent OnChanged;
        public override string __Usage => "Simple item slot for any use.";

        ItemSlotWithProcessor Slot
        {
            get
            {
                if (slot.IsDefault())
                    InitSlot();
                return slot;
            }
        }

        void InitSlot()
        {
            slot = new ItemSlotWithProcessor(i =>
            {
                if (MemberRequirement && !World.CategoryContains(MemberRequirement.ID, i.ID))
                    return Slot.Peek();
                if (CapacitySource)
                    if (World.GetReadOnlyAccessor<int>(CapacitySource).TryGetValue(i.ID, out var capacity)) 
                    {
                        capacity = Mathf.Min(capacity, MaximumCapacity);
                        return new ItemStack(i.ID, Mathf.Min(capacity, i.Value));
                    }

                return new ItemStack(i.ID, Mathf.Min(MaximumCapacity, i.Value));
            }); 
        }
        ItemSlotWithProcessor slot;
        

        public ItemStack Contained => Slot.Peek();
        public RuntimeID RequiredCategory { get; set; }

        void Awake()
        {
            RequiredCategory = MemberRequirement ? MemberRequirement.ID : default;
            
        }

        protected new void Start()
        {
            base.Start();
            if (!InitialObject?.ID.IsDefault() ?? false) InsertPossible(new ItemStack(InitialObject.ID, InitialObject.Value));
        }

        public event PolyChangeEvent Changed;

        public ItemStack RemainderIfInserted(ItemStack toInsert) => Slot.RemainderIfInserted(toInsert);


        public ItemStack InsertPossible(ItemStack toInsert)
        {
            var ret = Slot.InsertPossible(toInsert);
            if (!ret.Equals(toInsert))
                FireChange();
            return ret;
        }

        public ItemStack Peek()
        {
            return Slot.Peek();
        }

        public ItemStack Peek(Quantity arg)
        {
            return Slot.Peek(arg);
        }


        public ItemStack ExtractAmount(Quantity arg)
        {
            if (IsReadonly)
                return default;

            var ret = Slot.ExtractAmount(arg);
            if (!ret.IsDefault())
                FireChange();
            return ret;
        }


        public ItemStack ExtractAll()
        {
            if (IsReadonly)
                return default;

            var ret = Slot.ExtractAll();
            FireChange();
            return ret;
        }

        public bool CanExtract()
        {
            return !IsReadonly && Slot.CanExtract();
        }

        void FireChange()
        {
            Changed?.Invoke();
            OnChanged?.Invoke();
        }

        public void Discard()
        {
            Slot.ExtractAll();
        }
    }
}