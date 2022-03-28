using System.Collections.Generic;
using NUnit.Framework;
using Polyperfect.Crafting.Framework;

namespace Polyperfect.Crafting.Integration.Tests
{
    public class ItemDataTests
    {
        [Test]
        public void ItemDataRetrieval()
        {
            var item = new SimpleItem(RuntimeID.Random());
            var data = new NameData("Testing123");
            var manager = new Dictionary<RuntimeID, NameData> {[item.ID] = data}; //TypedDataManager<ID>();
            var retrieved = manager[item.ID].Name;
            Assert.AreEqual(retrieved, "Testing123");
        }

        [Test]
        public void SpecifiedMemberRetrieval()
        {
            var item = new SimpleItem(RuntimeID.Random());
            var categoryID = RuntimeID.Random();
            var data = new HashSet<RuntimeID>();

            var manager = new Dictionary<RuntimeID, HashSet<RuntimeID>> {[categoryID] = data}; //TypedDataManager<RuntimeID>();

            manager[categoryID].Add(item.ID);
            Assert.IsTrue(manager[categoryID].Contains(item.ID));
        }
    }
}