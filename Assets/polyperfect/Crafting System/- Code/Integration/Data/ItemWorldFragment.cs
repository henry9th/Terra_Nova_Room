using System.Collections.Generic;
using System.Linq;
using Polyperfect.Common;
using Polyperfect.Crafting.Framework;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Polyperfect.Crafting.Integration
{
    [CreateAssetMenu(menuName = "Polyperfect/Item World Fragment")]
    /// <summary>
    ///     A database for storing and accessing item data.
    /// </summary>
    public class ItemWorldFragment : PolyObject
    {
        public List<BaseObjectWithID> Objects = new List<BaseObjectWithID>();
        public override string __Usage => "Holds objects and all their data for use by other things.";
        public IEnumerable<BaseItemObject> ItemObjects => Objects.OfType<BaseItemObject>();
        public IEnumerable<BaseRecipeObject> RecipeObjects => Objects.OfType<BaseRecipeObject>();
        public IEnumerable<BaseCategoryObject> CategoryObjects => Objects.OfType<BaseCategoryObject>();


        protected void OnValidate()
        {
            #if UNITY_EDITOR
            var so = new SerializedObject(this);
            var arr = so.FindProperty(nameof(Objects));
            for (var i = Objects.Count-1; i >= 0; i--)
            {
                var prop = arr.GetArrayElementAtIndex(i);
                if (!prop.objectReferenceValue) 
                    arr.DeleteArrayElementAtIndex(i);
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(this);
            #endif
        }
    }

    public interface IItemWorld : IReadOnlyDataAccessorLookupWithArg<RuntimeID, RuntimeID>
    {
        bool CategoryContains(RuntimeID categoryId, RuntimeID itemID);
        IReadOnlyDictionary<RuntimeID, SimpleRecipe> Recipes { get; }
    }

    public static class ItemWorldExtensions
    {
        public static IReadOnlyDictionary<RuntimeID, string> GetNameLookup(this IItemWorld that)
        {
            var accessor = that.GetReadOnlyAccessor<string>(StaticCategories.Names);
            Assert.IsTrue(accessor!=null);
            return accessor;
        }

        public static string GetName(this IItemWorld that,RuntimeID id) =>
            that.GetNameLookup()[id];

        public static RuntimeID GetIDSlow(this IItemWorld that, string name)
        {
            var accessor = that.GetNameLookup();
            foreach (var item in accessor)
            {
                if (item.Value == name)
                    return item.Key;
            }

            throw new KeyNotFoundException();
        }
    }
}