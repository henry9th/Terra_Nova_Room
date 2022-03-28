using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Polyperfect.Crafting.Framework;
using Polyperfect.Crafting.Integration;
using UnityEngine;

namespace Polyperfect.Crafting.Integration
{
    public class SimpleItemWorld : IItemWorld,IDataAccessorLookupWithArg<RuntimeID,RuntimeID>
    {
        readonly Dictionary<RuntimeID, IDictionary> categoryDataLookups = new Dictionary<RuntimeID, IDictionary>();
        readonly Dictionary<RuntimeID, HashSet<RuntimeID>> categoryElements = new Dictionary<RuntimeID, HashSet<RuntimeID>>();
        readonly List<RuntimeID> items = new List<RuntimeID>();
        readonly List<RuntimeID> categories = new List<RuntimeID>();
        //readonly List<RuntimeID> recipes = new List<RuntimeID>();
        readonly Dictionary<RuntimeID, string> names = new Dictionary<RuntimeID, string>();
        readonly Dictionary<RuntimeID, SimpleRecipe> recipes = new Dictionary<RuntimeID, SimpleRecipe>();
        public bool CategoryContains(RuntimeID categoryId, RuntimeID itemID) => categoryElements[categoryId].Contains(itemID);

        public SimpleItemWorld()
        {
            categoryDataLookups.Add(StaticCategories.Names,names);
        }
        public IReadOnlyDictionary<RuntimeID, SimpleRecipe> Recipes => recipes;
        public IReadOnlyDictionary<RuntimeID, VALUE> GetReadOnlyAccessor<VALUE>(RuntimeID id)
        {
            if (categoryDataLookups.TryGetValue(id, out var contained))
            {
                if (!(contained is Dictionary<RuntimeID, VALUE> dictionary))
                    throw new ArrayTypeMismatchException($"The collection with ID {id} was not of the expected type {typeof(VALUE).Name}");
                return dictionary; //new ReadOnlyDictionary<RuntimeID, VALUE>(dictionary);
            }

            Debug.LogError($"No category has the id {id}");
            return null;
        }

        //public string GetName(RuntimeID id) => nameLookup[id];

        public IDictionary<RuntimeID, VALUE> GetAccessor<VALUE>(RuntimeID id)
        {
            if (categoryDataLookups.TryGetValue(id, out var contained))
            {
                if (!(contained is Dictionary<RuntimeID, VALUE> dictionary))
                    throw new ArrayTypeMismatchException($"The collection with ID {id} was not of the expected type {typeof(VALUE).Name}");
                return dictionary; //new ReadOnlyDictionary<RuntimeID, VALUE>(dictionary);
            }

            Debug.LogError($"No category has the id {id}");
            return null;
        }
        
        public void AddItem(RuntimeID item,string name)
        {
            items.Add(item);
            names[item] = name;
        }

        public void AddCategory(RuntimeID category, string name,IEnumerable<RuntimeID> elements = null)
        {
            categories.Add(category);
            var idLookup = new HashSet<RuntimeID>();
            categoryElements.Add(category,idLookup);
            if (elements !=null)
                    idLookup.UnionWith(elements);
            names[category] = name;
        }
        public void AddCategoryWithData(RuntimeID category,string name,IDictionary useOrAddDataLookup)
        {
            categories.Add(category);
            categoryElements.Add(category,new HashSet<RuntimeID>());
            if (!categoryDataLookups.ContainsKey(category))
                categoryDataLookups.Add(category,useOrAddDataLookup);
            else
            {
                foreach (DictionaryEntry item in useOrAddDataLookup)
                {
                    if (categoryDataLookups[category].Contains(item.Key))
                    {
                        Debug.LogError($"{name} already contained {item.Key}, skipping");
                        continue;
                    }
                    categoryDataLookups[category].Add(item.Key,item.Value);
                    categoryElements[category].Add((RuntimeID)item.Key);
                }
            }
            names[category] = name;
        }

        public void AddRecipe(RuntimeID id,SimpleRecipe recipe, string name)
        {
            recipes.Add(id,recipe);
            names[id] = name;
        }

        public void AddItemToCategory(RuntimeID categoryID, RuntimeID itemID)
        {
            categoryElements[categoryID].Add(itemID);
        }

        public void AddItemToCategory<DATA>(RuntimeID categoryID, RuntimeID itemID, DATA data)
        {
            categoryElements[categoryID].Add(itemID);
            if (categoryDataLookups.TryGetValue(categoryID, out var contained))
            {
                if (!(contained is Dictionary<RuntimeID, DATA> dictionary))
                    throw new ArrayTypeMismatchException($"The collection with ID {categoryID} was not of the expected type {typeof(DATA).Name}");
                dictionary[itemID] = data;
            }
        }


    }
}