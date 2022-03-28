using System;
using System.Collections.Generic;
using System.Linq;
using Polyperfect.Crafting.Framework;
using UnityEngine;
using UnityEngine.Events;

namespace Polyperfect.Crafting.Integration
{
    public class Crafter : ItemUserBase
    {
        public override string __Usage => "Exposes methods for crafting to Unity Events, for things like UGUI Buttons";
        [SerializeField] MatchingMode MatchType;

        public UnityEvent 
            OnRecipeChange = new UnityEvent(),
            OnSourceUpdate = new UnityEvent(), 
            OnDestinationUpdate = new UnityEvent();

        public SimpleRecipe Recipe
        {
            get => recipe;
            set
            {
                recipe = value;
                OnRecipeChange.Invoke();
            }
        }


        public BaseItemStackInventory Source
        {
            get => source;
            set
            {
                if (source is IChangeable changeableOld) 
                    changeableOld.Changed -= SendSourceUpdated;
                source = value;
                if (source is IChangeable changeableNew) 
                    changeableNew.Changed += SendSourceUpdated;
                OnSourceUpdate.Invoke();
            }
        }

        public BaseItemStackInventory Destination
        {
            get => destination;
            set
            {
                if (destination is IChangeable changeableOld) 
                    changeableOld.Changed -= SendDestinationUpdated;
                destination = value;
                if (destination is IChangeable changeableNew) 
                    changeableNew.Changed += SendDestinationUpdated;
                OnDestinationUpdate.Invoke();
            }
        }

        BaseItemStackInventory source;
        BaseItemStackInventory destination;
        SimpleRecipe recipe;
        void SendDestinationUpdated() => OnDestinationUpdate.Invoke();

        ISatisfier<IEnumerable<IValueAndID<Quantity>>, Quantity> Satisfier { get; set; }


        void Awake()
        {
            switch (MatchType)
            {
                case MatchingMode.AnyPosition:
                    Satisfier = new AnyPositionItemQuantitySatisfier<IValueAndID<Quantity>>();
                    break;
                case MatchingMode.SamePosition:
                    Satisfier = new MatchingPositionSatisfier<IValueAndID<Quantity>>(new QuantityAndIDSatisfier<IValueAndID<Quantity>>(), int.MaxValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void SetSource(GameObject go) => Source = go.GetComponentInChildren<BaseItemStackInventory>();
        public void ClearSource() => Source = null;
        public void SetDestination(GameObject go) => Destination = go.GetComponentInChildren<BaseItemStackInventory>();
        public void ClearDestination() => Destination = null;
        public void SetRecipe(BaseRecipeObject obj) => Recipe = new SimpleRecipe(obj.Ingredients, obj.Outputs);
        public void ClearRecipe() => Recipe = null;

        public void CraftAmount(int craftAmount)
        {
            // if (!CheckIfValid()) 
            //     Debug.LogError("Make sure that Source, Destination, and Recipe are all assigned to the Crafter on " + gameObject.name);
            craftAmount = Mathf.Min(craftAmount, GetMaxCraftAmount(Source, Recipe));
            if (craftAmount < 1)
                return;
            var factory = new MultiItemFactory(Recipe.Output);
            var ghostCreation = factory.Create(craftAmount).ToList();

            if (!InventoryOps.CanInsertCollectionCompletely(ghostCreation, Destination.GetSlots()))
                return;
            InventoryOps.ExtractCompletelyFromCollection(Source.GetSlots(), Recipe.Requirements.Multiply(craftAmount));
            InventoryOps.InsertCompletelyIntoCollection(ghostCreation, Destination.GetSlots());
        }


        public bool CheckIfValid()
        {
            return !(Source.IsDefault() || Destination.IsDefault() || Satisfier.IsDefault() || Recipe.IsDefault());
        }

        public int GetMaxCraftAmount(BaseItemStackInventory inventory, SimpleRecipe testRecipe)
        {
            if (testRecipe.IsDefault() || inventory.IsDefault())
                return 0;
            var satisfier = Satisfier;
            var requirements = testRecipe.Requirements.Cast<IValueAndID<Quantity>>();
            var suppliedItems = inventory.GetItems().Cast<IValueAndID<Quantity>>();
            var maxCraftAmount = satisfier.SatisfactionWith(requirements, suppliedItems);
            return maxCraftAmount;
        }

        enum MatchingMode
        {
            AnyPosition,
            SamePosition
        }

        void SendSourceUpdated()
        {
            OnSourceUpdate.Invoke();
        }
    }
}