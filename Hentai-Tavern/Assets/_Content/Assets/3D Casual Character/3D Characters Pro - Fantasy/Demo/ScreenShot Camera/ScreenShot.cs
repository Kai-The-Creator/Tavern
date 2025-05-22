using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Layer_lab._3D_Casual_Character.Fantasy
{
    public class ScreenShot : MonoBehaviour
    {
        public static ScreenShot Instance;

        private void Awake()
        {
            Instance = this;
        }

        public string saveFolderPath;
        
        public async Task ScreenShotClickAsync(string screenShotName)
        {
            var cam = GetComponent<Camera>();
            var renderTexture = cam.targetTexture;

            RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.clear);

            cam.Render();

            var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();

            // 비동기 저장
            await SaveToFileAsync(texture, screenShotName);

            Debug.Log($"Screenshot for {screenShotName} saved!");
        }

        private async Task SaveToFileAsync(Texture2D texture, string screenShotName)
        {
            var filePath = $"{saveFolderPath}/{screenShotName}.png";
            var imageData = texture.EncodeToPNG();

            // 파일 비동기 저장
            await Task.Run(() => File.WriteAllBytes(filePath, imageData));
        }

    }
}