using System.Linq;
using Polyperfect.Common.Edit;
using Polyperfect.Crafting.Integration;
using UnityEditor;

namespace Polyperfect.Crafting.Edit
{
    public class ItemWorldAutoUpdater : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var any = false;
            foreach (var path in importedAssets)
            {
                var atPath = AssetDatabase.LoadMainAssetAtPath(path);
                if (atPath is BaseObjectWithID)
                {
                    any = true;
                    break;
                }
            }

            if (!any)
                return;
            var autoWorld = AssetUtility.FindAssetsOfType<ItemWorldFragment>().FirstOrDefault(o => o.name.Contains("_AutoItemWorld"));
            if (!autoWorld)
                return;

            autoWorld.Objects.Clear();
            autoWorld.Objects.AddRange(AssetUtility.FindAssetsOfType<BaseObjectWithID>());
            EditorUtility.SetDirty(autoWorld);
            EditorApplication.delayCall += AssetDatabase.SaveAssets;
        }
    }
}