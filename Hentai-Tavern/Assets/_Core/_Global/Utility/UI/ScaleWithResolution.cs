using UnityEngine;

public class ScaleWithResolution : MonoBehaviour
{
    // Минимальное и максимальное разрешение, для которых будет рассчитываться масштаб
    public Vector2 minResolution = new Vector2(640, 360);
    public Vector2 maxResolution = new Vector2(1920, 1080);

    // Минимальный и максимальный масштаб объекта
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 maxScale = new Vector3(2.0f, 2.0f, 2.0f);

    void Start()
    {
        // Получаем текущее разрешение экрана
        float currentWidth = Screen.width;
        float currentHeight = Screen.height;

        // Рассчитываем коэффициент масштабирования по ширине и высоте
        float widthRatio = (currentWidth - minResolution.x) / (maxResolution.x - minResolution.x);
        float heightRatio = (currentHeight - minResolution.y) / (maxResolution.y - minResolution.y);

        // Берем среднее значение между коэффициентами ширины и высоты
        float scaleRatio = (widthRatio + heightRatio) / 2.0f;

        // Ограничиваем коэффициент масштабирования в пределах [0, 1]
        scaleRatio = Mathf.Clamp(scaleRatio, 0.0f, 1.0f);

        // Рассчитываем новый масштаб объекта
        Vector3 newScale = Vector3.Lerp(minScale, maxScale, scaleRatio);

        // Применяем новый масштаб к объекту
        transform.localScale = newScale;
    }
}