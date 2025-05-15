using UnityEngine;

namespace OnGame.Utils
{
    public class Helper
    {
        public static T GetComponent_Helper<T>(GameObject go) where T : Component
        {
            if (go == null)
            {
                Debug.LogError("Warning! : GameObject is null!");
                return null;
            }

            var result = go.TryGetComponent<T>(out var component) ? component : null;
            if(result == null){Debug.LogError($"Warning! : {typeof(T)} is missing in {go.name}!");}
            return result;
        }
        
        public static T GetComponentInChildren_Helper<T>(GameObject go) where T : Component
        {
            if (go == null)
            {
                Debug.LogError("Warning! : GameObject is null!");
                return null;
            }

            var result = go.GetComponentInChildren<T>();
            if(result == null){Debug.LogError($"Warning! : {typeof(T)} in children of {go.name} is missing!");}
            return result;
        }
        
        public static T GetComponentInChildren_Helper<T>(GameObject go, bool includeInactive) where T : Component
        {
            if (go == null)
            {
                Debug.LogError("Warning! : GameObject is null!");
                return null;
            }
            
            var result = go.GetComponentInChildren<T>(includeInactive);
            if(result == null){Debug.LogError($"Warning! : {typeof(T)} in children of {go.name} is missing!");}
            return result;
        }
        
        public static T GetComponentInParent_Helper<T>(GameObject go) where T : Component
        {
            if (go == null)
            {
                Debug.LogError("Warning! : GameObject is null!");
                return null;
            }
            
            var result = go.GetComponentInParent<T>();
            if(result == null){Debug.LogError($"Warning! : {typeof(T)} in parent of {go.name} is missing!");}
            return result;
        }

        public static T GetComponentInParent_Helper<T>(GameObject go, bool includeInactive) where T : Component
        {
            if (go == null)
            {
                Debug.LogError("Warning! : GameObject is null!");
                return null;
            }
            
            var result = go.GetComponentInParent<T>(includeInactive);
            if(result == null){Debug.LogError($"Warning! : {typeof(T)} in parent of {go.name} is missing!");}
            return result;
        }
    }
}
