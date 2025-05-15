using OnGame.Contents.Items.Base;
using UnityEngine;

namespace OnGame.Contents.Items.Data
{
    public class PartsData : ItemData
    {
        public override ItemBase Create()
        {
            var parts = new PartsBase { data = this };
            return parts;
        }
    }
}
