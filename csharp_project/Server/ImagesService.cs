using Google.Protobuf;
using Grpc.Net.Client;
using TradingForumGrpc;
using ImageDTO = ApiContracts.ImageDTO;

namespace Domain;

public class ImagesService : IImagesService{
    private ImageService.ImageServiceClient Client { get; set; }
    
    public ImagesService(ImageService.ImageServiceClient client)
    {
        Client = client;
    }

    public async Task<ImageDTO> GetImageAsync(int id){
        TradingForumGrpc.ImageDTO grpcImageDto = await Client.getImageAsync(new TradingForumGrpc.ImageId(){ Id = id });
        return new ImageDTO(){ Id = grpcImageDto.Id, Data = grpcImageDto.Data.Memory.ToArray() };
    }

    public async Task<ImageDTO> AddImageAsync(int postId, byte[] data){
        if (!IsValidImage(data) ){
            throw new Exception("Kun billede formater er tilladt");
        }

        int id = (await Client.addImageAsync(new AddImageRequest(){ PostId = postId, Data = ByteString.CopyFrom(data) })).Id;
        return new ImageDTO(){ Id = id };
    }

    public bool IsValidImage(byte[] data){
        return IsJpeg(data) || IsPng(data) || IsWebp(data);
    }

    public bool IsJpeg(byte[] data){
        return data.Take(3)
            .SequenceEqual(new byte[]{ 0xff, 0xD8, 0xff }); // jpeg identifier
    }

    public bool IsPng(byte[] data){
        return data.Take(8)
            .SequenceEqual(new byte[]{ 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a }); // png identifier
    }

    public bool IsWebp(byte[] data){
        return data.Take(4)
                   .SequenceEqual(new byte[]{ 0x52, 0x49, 0x46, 0x46 })// webp identifier 1st half
               && data.Skip(8).Take(7)
                   .SequenceEqual(new byte[]{ 0x57, 0x45, 0x42, 0x50, 0x56, 0x50, 0x38 }); // webp identifier 2nd half
    }
}