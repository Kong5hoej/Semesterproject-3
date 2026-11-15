using ApiContracts;

namespace Domain;

public interface IImagesService{
    Task<ImageDTO> GetImageAsync(int id);
    Task<ImageDTO> AddImageAsync(int postId, byte[] data);
    bool IsValidImage(byte[] data);
    bool IsJpeg(byte[] data);
    bool IsPng(byte[] data);
    bool IsWebp(byte[] data);
}