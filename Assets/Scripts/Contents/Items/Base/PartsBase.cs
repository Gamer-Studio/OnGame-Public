using System;
using OnGame.Contents.Items;
using OnGame.Scenes.World;

namespace OnGame.Contents.Items.Base
{
    [Serializable]
    public class PartsBase : ItemBase, IActive, IPassive
    {
        private IActive activeImpl;
        private IPassive passiveImpl;

        public void SetActiveImpl(IActive active)
        {
            activeImpl = active;
        }
        public void SetPassiveImpl(IPassive passive)
        {
            passiveImpl = passive;
        }
        public void Shoot() //active
        {
            if (activeImpl != null)
                activeImpl.Shoot();
        }

        public void Use() //active
        {
            if (activeImpl != null)
                activeImpl.Use();
        }
        public void Apply(Player player) //passive
        {
            if (passiveImpl != null)
                passiveImpl.Apply(player);
        }

        public void Remove(Player player)
        {
            if (passiveImpl != null)
                passiveImpl.Remove(player);
        }
        
        public void AddPart<T>(T part) where T : ItemBase, IPassive
        {
            if (activeImpl != null)
                activeImpl.AddPart(part);
        }

        public void RemovePart<T>(T part) where T : ItemBase, IPassive
        {
            if (activeImpl != null)
                activeImpl.RemovePart(part);
        }

        public ItemBase[] GetParts()
        {
            if (activeImpl != null)
                return activeImpl.GetParts();
            else
                return new ItemBase[0];
        }
        public bool ConsumeOnUse
        {
            get => activeImpl?.ConsumeOnUse ?? false;
            set { if (activeImpl != null) activeImpl.ConsumeOnUse = value; }
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (passiveImpl != null)
                return passiveImpl.TryGetValue(key, out value);

            value = default;
            return false;
        }
    }
}
