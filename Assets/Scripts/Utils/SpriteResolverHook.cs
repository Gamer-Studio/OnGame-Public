using UnityEngine;
using UnityEngine.U2D.Animation;

namespace OnGame.Utils
{
    /// <summary>
    /// SpriteLibrary, Sprite Resolver, Animator 이용해서 애니메이션 구현할 떄 사용하는 훅
    /// </summary>
    public class SpriteResolverHook : MonoBehaviour
    {
        [SerializeField] private SpriteResolver resolver;

        private void Awake()
        {
            if (resolver == null)
            {
                Debug.LogError($"{name}: SpriteResolver가 연결되지 않았습니다!");
            }
        }

        public string SpriteLabel
        {
            get => resolver != null ? resolver.GetLabel() : string.Empty;
            set
            {
                if (resolver == null) return;
                resolver.SetCategoryAndLabel(resolver.GetCategory(), value);
            }
        }

        public void SetCategory(string category)
        {
            if (resolver == null) return;
            resolver.SetCategoryAndLabel(category, resolver.GetLabel());
        }
        
        // Category와 Label을 동시에 강제 지정
        public void SetCategoryAndLabel(string category, string label)
        {
            if (resolver == null) return;
            resolver.SetCategoryAndLabel(category, label);
        }
    }
}