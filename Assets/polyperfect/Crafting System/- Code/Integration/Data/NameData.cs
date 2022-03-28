using System;

namespace Polyperfect.Crafting.Integration
{
    [Serializable]
    public struct NameData
    {
        public string Name;

        public NameData(string name)
        {
            Name = name;
        }
    }
}