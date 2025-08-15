using ProjectSlam.Data.Interfaces;
using System.IO;
using System.Windows.Media.Imaging;

namespace ProjectSlam.Data.Services;

public class PhotoService : IPhotoService
{
    private const int MaxPhotoSizeBytes = 5 * 1024 * 1024; // 5MB
    private readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png" };
    private readonly string PhotoStoragePath;

    public PhotoService(string photoStoragePath)
    {
        PhotoStoragePath = photoStoragePath;
        Directory.CreateDirectory(photoStoragePath);
    }

    public async Task<(byte[] data, string mimeType)?> LoadPhotoAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return null;

            var bytes = await File.ReadAllBytesAsync(filePath);
            var mimeType = GetMimeType(filePath);

            if (!ValidatePhoto(bytes, mimeType))
                return null;

            return (bytes, mimeType);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public bool ValidatePhoto(byte[] photoData, string mimeType)
    {
        if (photoData == null || photoData.Length == 0 || photoData.Length > MaxPhotoSizeBytes)
            return false;

        if (!AllowedMimeTypes.Contains(mimeType?.ToLower()))
            return false;

        try
        {
            using var stream = new MemoryStream(photoData);
            var decoder = BitmapDecoder.Create(
                stream,
                BitmapCreateOptions.None,
                BitmapCacheOption.None);

            return decoder != null && decoder.Frames.Count > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> SavePhotoAsync(byte[] photoData, string mimeType, string offenderId)
    {
        if (string.IsNullOrWhiteSpace(offenderId))
            throw new ArgumentException("Offender ID cannot be null or empty", nameof(offenderId));

        if (!ValidatePhoto(photoData, mimeType))
            throw new ArgumentException("Invalid photo data or mime type");

        var extension = mimeType.Split('/')[1];
        var fileName = $"{offenderId}_{DateTime.Now:yyyyMMddHHmmss}.{extension}";
        var filePath = Path.Combine(PhotoStoragePath, fileName);

        await File.WriteAllBytesAsync(filePath, photoData);
        return filePath;
    }
    public string GetMimeType(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return string.Empty;

        var extension = Path.GetExtension(filePath).ToLower();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => string.Empty
        };
    }
}
