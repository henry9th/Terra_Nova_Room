using System.Collections;
using System.Collections.Generic;
using Polyperfect.Crafting.Framework;
using Polyperfect.Crafting.Integration;
using UnityEngine;

namespace Polyperfect.Common.Edit
{
    public class EditorItemNameAccessor : IReadOnlyDictionary<RuntimeID, NameData>
    {
        readonly Dictionary<RuntimeID, NameData> names = new Dictionary<RuntimeID, NameData>();

        public EditorItemNameAccessor()
        {
            foreach (var item in AssetUtility.FindAssetsOfType<BaseObjectWithID>())
            {
                if (names.ContainsKey(item.ID))
                {
                    Debug.LogError($"The ID already exists for item {item.name}. Randomizing the ID. If this happens again after doing something specific, please report a bug.");
                    item.RandomizeID();
                }
                names.Add(item.ID, new NameData(item.name));
            }
        }

        public bool TryGetValue(RuntimeID id, out NameData data)
        {
            if (ContainsKey(id))
            {
                data = this[id];
                return true;
            }

            data = default;
            return false;
        }

        public bool ContainsKey(RuntimeID id)
        {
            return names.ContainsKey(id);
        }

        public NameData this[RuntimeID key] => names[key];
        public IEnumerable<RuntimeID> Keys => names.Keys;
        public IEnumerable<NameData> Values => names.Values;

        public IEnumerator<KeyValuePair<RuntimeID, NameData>> GetEnumerator()
        {
            return names.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return names.GetEnumerator();
        }

        public int Count => names.Count;
    }
}