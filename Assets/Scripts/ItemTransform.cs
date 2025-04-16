using UnityEngine;

public class ItemTransform : MonoBehaviour
{
    // 缩放相关参数
    public float scaleSensitivity = 0.5f;  // 缩放速度
    public float minScale = 0.1f;          // 最小缩放值
    public float maxScale = 5.0f;          // 最大缩放值

    // 旋转相关参数
    public float rotationSensitivity = 0.2f; // 旋转敏感度
    private bool isRotating = false;         // 是否正在旋转
    private Vector3 lastMousePosition;       // 上一次鼠标位置

    void Update()
    {
        // 获取当前选中的物体
        ChildItem selectedItem = SelectionManager.Instance.GetSelectedItem();
        if (selectedItem == null)
            return;

        Transform targetTransform = selectedItem.transform;

        // ----- 滚轮缩放操作 -----
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollInput) > 0.001f)
        {
            Vector3 scaleChange = Vector3.one * scrollInput * scaleSensitivity;
            Vector3 currentScale = targetTransform.localScale;

            // 如果使用旋转翻转，localScale 不会改变符号（除非其他逻辑修改），这里直接加上 scaleChange
            Vector3 newScale = currentScale + scaleChange;
            newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
            newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
            newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);
            targetTransform.localScale = newScale;
        }

        // ----- 右键拖拽旋转操作 -----
        if (Input.GetMouseButtonDown(1))
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1) && isRotating)
        {
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 delta = currentMousePos - lastMousePosition;
            // 基础旋转量：水平位移加上(负的)垂直位移
            float rotationAmount = (delta.x - delta.y) * rotationSensitivity;

            // 根据翻转状态调整旋转方向
            // 计算一个翻转因子：如果水平翻转则乘 -1，若竖直翻转也乘 -1
            int flipFactor = 1;
            if (selectedItem.isFlippedHorizontally)
                flipFactor *= -1;
            if (selectedItem.isFlippedVertically)
                flipFactor *= -1;

            // 使用局部坐标系旋转，注意旋转轴这里使用 Z 轴
            targetTransform.Rotate(0, 0, -rotationAmount * flipFactor, Space.Self);

            lastMousePosition = currentMousePos;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }
    }
}
