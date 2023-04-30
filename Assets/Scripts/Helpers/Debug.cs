using UnityEngine;
using UnityEngine.UI;

public class Debug : MonoBehaviour {

    private static Debug main;
    [SerializeField] private InputField debugField;

    private void Awake() {
        main = this;
    }

    public static void Log(object value) {
        UnityEngine.Debug.Log(value);
        if (main != null) main.ConsoleWrite(value);
    }
    public static void Log(object value, Object context) {
        UnityEngine.Debug.Log(value, context);
        if (main != null) main.ConsoleWrite(value);
    }
    public static void LogWarning(object value) {
        UnityEngine.Debug.LogWarning(value);
        if (main != null) main.ConsoleWrite(value);
    }
    public static void LogWarning(object value, Object context) {
        UnityEngine.Debug.LogWarning(value, context);
        if (main != null) main.ConsoleWrite(value);
    }

    public static void DrawLine(Vector3 start, Vector3 end) {
        UnityEngine.Debug.DrawLine(start, end);
    }
    public static void DrawLine(Vector3 start, Vector3 end, Color color) {
        UnityEngine.Debug.DrawLine(start, end, color);
    }
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration) {
        UnityEngine.Debug.DrawLine(start, end, color, duration);
    }
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest) {
        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
    }

    protected void ConsoleWrite(object value) {
        debugField.text += "\n" + value;
        ResizeField();
    }

    private void ResizeField() {
        //if (gameObject.activeSelf)
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(debugField.GetComponent<RectTransform>());
    }

    public void Clear() {
        debugField.text = "";
        main.ResizeField();
    }

}
