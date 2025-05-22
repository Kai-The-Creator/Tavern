using UnityEngine;

public class ScaleWithResolution : MonoBehaviour
{
    // ����������� � ������������ ����������, ��� ������� ����� �������������� �������
    public Vector2 minResolution = new Vector2(640, 360);
    public Vector2 maxResolution = new Vector2(1920, 1080);

    // ����������� � ������������ ������� �������
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 maxScale = new Vector3(2.0f, 2.0f, 2.0f);

    void Start()
    {
        // �������� ������� ���������� ������
        float currentWidth = Screen.width;
        float currentHeight = Screen.height;

        // ������������ ����������� ��������������� �� ������ � ������
        float widthRatio = (currentWidth - minResolution.x) / (maxResolution.x - minResolution.x);
        float heightRatio = (currentHeight - minResolution.y) / (maxResolution.y - minResolution.y);

        // ����� ������� �������� ����� �������������� ������ � ������
        float scaleRatio = (widthRatio + heightRatio) / 2.0f;

        // ������������ ����������� ��������������� � �������� [0, 1]
        scaleRatio = Mathf.Clamp(scaleRatio, 0.0f, 1.0f);

        // ������������ ����� ������� �������
        Vector3 newScale = Vector3.Lerp(minScale, maxScale, scaleRatio);

        // ��������� ����� ������� � �������
        transform.localScale = newScale;
    }
}