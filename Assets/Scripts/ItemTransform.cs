using UnityEngine;

public class ItemTransform : MonoBehaviour
{
    // ������ز���
    public float scaleSensitivity = 0.5f;  // �����ٶ�
    public float minScale = 0.1f;          // ��С����ֵ
    public float maxScale = 5.0f;          // �������ֵ

    // ��ת��ز���
    public float rotationSensitivity = 0.2f; // ��ת���ж�
    private bool isRotating = false;         // �Ƿ�������ת
    private Vector3 lastMousePosition;       // ��һ�����λ��

    void Update()
    {
        // ��ȡ��ǰѡ�е�����
        ChildItem selectedItem = SelectionManager.Instance.GetSelectedItem();
        if (selectedItem == null)
            return;

        Transform targetTransform = selectedItem.transform;

        // ----- �������Ų��� -----
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollInput) > 0.001f)
        {
            Vector3 scaleChange = Vector3.one * scrollInput * scaleSensitivity;
            Vector3 currentScale = targetTransform.localScale;

            // ���ʹ����ת��ת��localScale ����ı���ţ����������߼��޸ģ�������ֱ�Ӽ��� scaleChange
            Vector3 newScale = currentScale + scaleChange;
            newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
            newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
            newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);
            targetTransform.localScale = newScale;
        }

        // ----- �Ҽ���ק��ת���� -----
        if (Input.GetMouseButtonDown(1))
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1) && isRotating)
        {
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 delta = currentMousePos - lastMousePosition;
            // ������ת����ˮƽλ�Ƽ���(����)��ֱλ��
            float rotationAmount = (delta.x - delta.y) * rotationSensitivity;

            // ���ݷ�ת״̬������ת����
            // ����һ����ת���ӣ����ˮƽ��ת��� -1������ֱ��תҲ�� -1
            int flipFactor = 1;
            if (selectedItem.isFlippedHorizontally)
                flipFactor *= -1;
            if (selectedItem.isFlippedVertically)
                flipFactor *= -1;

            // ʹ�þֲ�����ϵ��ת��ע����ת������ʹ�� Z ��
            targetTransform.Rotate(0, 0, -rotationAmount * flipFactor, Space.Self);

            lastMousePosition = currentMousePos;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }
    }
}
