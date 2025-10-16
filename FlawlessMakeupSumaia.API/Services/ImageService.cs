namespace FlawlessMakeupSumaia.API.Services
{
    public interface IImageService
    {
        string ProcessImage(string imageData);
        bool IsBase64Image(string imageData);
    }

    public class ImageService : IImageService
    {
        public string ProcessImage(string imageData)
        {
            if (string.IsNullOrWhiteSpace(imageData))
                return string.Empty;

            // If it's already a URL (http:// or https://), return as is
            if (imageData.StartsWith("http://") || imageData.StartsWith("https://"))
                return imageData;

            // If it's base64 data, ensure it has the data URI prefix
            if (IsBase64Image(imageData))
            {
                // If it already has data:image prefix, return as is
                if (imageData.StartsWith("data:image/"))
                    return imageData;

                // Otherwise, add the prefix (assuming JPEG)
                return $"data:image/jpeg;base64,{imageData}";
            }

            return imageData;
        }

        public bool IsBase64Image(string imageData)
        {
            if (string.IsNullOrWhiteSpace(imageData))
                return false;

            // Check if it starts with data:image/
            if (imageData.StartsWith("data:image/"))
                return true;

            // Check if it's a valid base64 string (simple check)
            // Base64 strings are typically long and contain only valid base64 characters
            if (imageData.Length > 100 && !imageData.Contains("http"))
            {
                try
                {
                    // Try to extract base64 data if it has the data URI prefix
                    var base64Data = imageData.Contains(",") ? imageData.Split(',')[1] : imageData;
                    Convert.FromBase64String(base64Data);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
    }
}

