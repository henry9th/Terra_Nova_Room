using System.Collections.Generic;
using Polyperfect.Common;
using Polyperfect.Crafting.Framework;
using UnityEngine;

namespace Polyperfect.Crafting.Integration.Demo
{
    [RequireComponent(typeof(ItemSlotComponent))]
    public class DisplayItemModel : ItemUserBase
    {
        public override string __Usage => "Creates a prefab based on the specified data for a given item, and sizes it so it fits within the given bounds. Connects to the attached ItemSlotComponent automatically.";
        public float MaxSize = 1f;
        [SerializeField] CategoryWithPrefab DataSource;
        [SerializeField] Transform ModelParent;
        static readonly Dictionary<GameObject, Bounds> prefabBounds = new Dictionary<GameObject, Bounds>();
        public ItemStackEvent OnModelSucceeded = new ItemStackEvent(), OnModelFailed = new ItemStackEvent();
        public bool AutoRemovePhysicsComponents = true;
        GameObject created;
        void Awake()
        {
            if (!ModelParent)
                ModelParent = transform;
            var itemSlotComponent = GetComponent<ItemSlotComponent>();
            itemSlotComponent.Changed += () => CreateAndDisplay(itemSlotComponent.Peek());
        }

        public void CreateAndDisplay(ItemStack itemStack)
        {
            CleanupExistingModel();

            var accessor = World.GetReadOnlyAccessor<GameObject>(DataSource);
            if (!accessor.TryGetValue(itemStack.ID, out var prefab))
            {
                OnModelFailed.Invoke(itemStack);
                return;
            }

            CreateModel(prefab);
            OnModelSucceeded.Invoke(itemStack);
        }

        void CleanupExistingModel()
        {
            if (created)
                Destroy(created);
            created = null;
        }

        void CreateModel(GameObject prefab)
        {
            var inst = InstantiateAndAutoScaleModel(prefab);

            if (AutoRemovePhysicsComponents)
            {
                foreach (var item in inst.GetComponentsInChildren<Collider>())
                    DestroyImmediate(item);
                foreach (var item in inst.GetComponentsInChildren<Rigidbody>())
                    DestroyImmediate(item);
            }

            created = inst;
        }

        GameObject InstantiateAndAutoScaleModel(GameObject prefab)
        {
            var inst = Instantiate(prefab, ModelParent);

            var size = GetPrefabBounds(prefab).size;
            var max = Mathf.Max(size.x, size.y, size.z);
            var mul = MaxSize / max;

            inst.transform.localScale *= mul;
            return inst;
        }

        Bounds GetPrefabBounds(GameObject prefab)
        {
            if (prefabBounds.TryGetValue(prefab, out var bounds))
                return bounds;
            foreach (var item in prefab.GetComponentsInChildren<Renderer>()) 
                bounds.Encapsulate(item.bounds);
            prefabBounds[prefab] = bounds;
            return bounds;
        }
    }
}