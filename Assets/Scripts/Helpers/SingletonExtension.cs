using UnityEngine;

public static class SingletonExtension {
    public static bool InitializeSingleton<T>(this T _this, ref T _main) where T : MonoBehaviour {
        if (_main != null) {
            if (_main == _this)
                return true;
            MonoBehaviour.Destroy(_this.gameObject);
            return false;
        }
        _main = _this;
        if (Application.isPlaying) {
            _this.transform.SetParent(null);
            MonoBehaviour.DontDestroyOnLoad(_this.gameObject);
        }
        return true;
    }

}
