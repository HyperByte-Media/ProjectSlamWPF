namespace ProjectSlam.Data.Interfaces;

public interface IPhotoService
{
    Task<(byte[] data, string mimeType)?> LoadPhotoAsync(string filePath);
    bool ValidatePhoto(byte[] photoData, string mimeType);
    Task<string> SavePhotoAsync(byte[] photoData, string mimeType, string offenderId);
    string GetMimeType(string filePath);
}
