using ApiContracts;
using Domain.State;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components.Sections;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.OpenApi.Expressions;
using TradingForumGrpc;
using CategoryDTO = ApiContracts.CategoryDTO;
using PostDTO = ApiContracts.PostDTO;

namespace Domain;

public class PostsService : IPostsService{
    private PostService.PostServiceClient Client{ get; set; }
    private IImagesService ImagesService{ get; set; }
    private INotificationService NotificationService{ get; set; }
    private IUserService UserService{ get; set; }

    public PostsService(PostService.PostServiceClient client, IImagesService imagesService, INotificationService notificationService, IUserService userService){
        this.Client = client;
        this.ImagesService = imagesService;
        this.NotificationService = notificationService;
        this.UserService = userService;
    }

    public async Task<PostDTO> MakePost(CreatePostDTO createPostDto){
        if (createPostDto.ZipCode.Length != 4)
            throw new Exception("Postnummeret er ikke godkendt");
        if (createPostDto.ZipCode.Any(c => !Char.IsAsciiDigit(c)))
            throw new Exception("Postnummeret må kun bestå af tal");
        
        if (string.IsNullOrEmpty(createPostDto.Title) || string.IsNullOrEmpty(createPostDto.Description) || createPostDto.Price < 0)
            throw new Exception("Din titel, beskrivelse eller pris er forkert!");

        if (createPostDto.Images.Count > 8){
            throw new Exception("Du må maks have 8 billeder");
        }
        
        List<byte[]> rawImages = new List<byte[]>();
        foreach (string image in createPostDto.Images){
            byte[] rawImage = Convert.FromBase64String(image);
            if (!ImagesService.IsValidImage(rawImage)){
                throw new Exception("Kun billede formater er tilladt");
            }
            rawImages.Add(rawImage);
        }

        List<TradingForumGrpc.CategoryDTO> categoryDtos = new List<TradingForumGrpc.CategoryDTO>();
        for (int i = 0; i < createPostDto.Categories.Count(); i++)
        {
            categoryDtos.Add(new TradingForumGrpc.CategoryDTO(){Name = createPostDto.Categories[i].Name});
        }
        
        CreatePostDTORequest request = new CreatePostDTORequest{ Title = createPostDto.Title, Description = createPostDto.Description, Price = createPostDto.Price, ZipCode = createPostDto.ZipCode, WantsToTrade = createPostDto.WantsToTrade, Creator = createPostDto.Creator, Categories = { categoryDtos }};
        TradingForumGrpc.PostDTO postDto = Client.createPost(request).PostDto; //Sender afsted til javaserver
        PostDTO realPostDto = postDtoFromGrpc(postDto);
        
        foreach (byte[] rawImage in rawImages){
            int id = (await ImagesService.AddImageAsync(postDto.Id, rawImage)).Id ?? -1;
            if (id != -1){
                realPostDto.ImageIds.Add(id);
            }
        }
        
        return realPostDto;
    }

    public async Task ChangePostState(ChangePostStateDTO changePostStateDto)
    {
        TradingForumGrpc.PostDTO response = Client.getPost(new GetPostRequest{Id = changePostStateDto.postId}).Post;
        Post post = new Post(response.Title, response.Description, response.Price, response.ZipCode, response.WantsToTrade, response.Timestamp);
        post.SetState((PostState)response.State);

        if (changePostStateDto.makeActive) post.Activate();
        else post.MakeInactive();
        
        TradingForumGrpc.PostDTO postDto = new TradingForumGrpc.PostDTO{Title = post.Title, Description = post.Description, Price = post.Price, ZipCode = post.ZipCode, WantsToTrade = post.WantsToTrade,  SoldTo = post.SoldTo, Id = response.Id, State = (int)post.GetState(), Owner = response.Owner, Categories = { response.Categories },ImageIds = { response.ImageIds }, Timestamp = post.Timestamp};
        TradingForumGrpc.PostDTO updatedPost = await Client.editPostAsync(new EditPostRequest{Id = response.Id, Post = postDto});
        await SendNotifications(updatedPost);
    }

    public async Task ClosePost(ClosePostDTO closePostDto)
    {
        TradingForumGrpc.PostDTO response = Client.getPost(new GetPostRequest { Id = closePostDto.postId }).Post;
        Post post = new Post(response.Title, response.Description, response.Price, response.ZipCode,
            response.WantsToTrade, response.Timestamp);
        post.SetState((PostState)response.State);

        post.Close(closePostDto.soldTo);

        TradingForumGrpc.PostDTO postDto = new TradingForumGrpc.PostDTO
            {
                Title = post.Title, Description = post.Description, Price = post.Price, ZipCode = post.ZipCode,
                WantsToTrade = post.WantsToTrade, SoldTo = post.SoldTo, Id = response.Id, State = (int)post.GetState(),
                Owner = response.Owner, Categories = { response.Categories }, Timestamp = response.Timestamp, ImageIds = { response.ImageIds }
    };

        TradingForumGrpc.PostDTO updatedPost = await Client.editPostAsync(new EditPostRequest{Id = response.Id, Post = postDto});
        await SendNotifications(updatedPost);
    }

    public async Task ChangePost(PostDTO postDto){
        TradingForumGrpc.PostDTO old = Client.getPost(new GetPostRequest{Id = postDto.Id}).Post;
        if (old.State == 2){
            throw new Exception("Opslaget er lukket og kan ikke ændres");
        }
        
        if (postDto.ZipCode.Length != 4)
            throw new Exception("Postnummeret er ikke godkendt");
        if (postDto.ZipCode.Any(c => !Char.IsAsciiDigit(c)))
            throw new Exception("Postnummeret må kun bestå af tal");
        
        if (String.IsNullOrEmpty(postDto.Title))
            throw new Exception("Titlen må ikke være tom!");
        if (String.IsNullOrEmpty(postDto.Description))
            throw new Exception("Beskrivelsen må ikke være tom!");
        if (postDto.Price < 0)
            throw new Exception("Prisen må ikke være negativ!");
            
        TradingForumGrpc.PostDTO postDtoEdit = new TradingForumGrpc.PostDTO{Title = postDto.Title, Description = postDto.Description, Price = postDto.Price, ZipCode = postDto.ZipCode, WantsToTrade = postDto.WantsToTrade,  SoldTo = postDto.SoldTo, Id = postDto.Id, State = postDto.State, Owner = postDto.Owner, Categories = { postDto.Categories.Select(c => new TradingForumGrpc.CategoryDTO(){Name = c.Name}).ToList()}, ImageIds = { postDto.ImageIds.Select(i => new ImageId(){Id = i}) }, Timestamp = postDto.Timestamp};

        TradingForumGrpc.PostDTO updatedPost = await Client.editPostAsync(new EditPostRequest{Id = postDto.Id, Post = postDtoEdit});
        await SendNotifications(updatedPost);
    }

    public List<PostDTO> GetPosts(){
        TradingForumGrpc.GetPostsResponse response = Client.getAllPosts(new Empty());

        List<PostDTO> posts = response.Post
            .Select(postDtoFromGrpc)
            .ToList();
        return posts;
    }

    public PostDTO GetPost(int postId){
        TradingForumGrpc.PostDTO response = Client.getPost(new GetPostRequest{ Id = postId }).Post;
        return postDtoFromGrpc(response);
    }

    private PostDTO postDtoFromGrpc(TradingForumGrpc.PostDTO postDto){
        return new PostDTO{
            Id = postDto.Id, 
            Title = postDto.Title, 
            Description = postDto.Description, 
            Price = postDto.Price, 
            ZipCode = postDto.ZipCode, 
            WantsToTrade = postDto.WantsToTrade, 
            State = postDto.State, 
            SoldTo = postDto.SoldTo, 
            Owner = postDto.Owner, 
            Categories = postDto.Categories.Select(c => new CategoryDTO(){ Name = c.Name }).ToList(),
            ImageIds = postDto.ImageIds.Select(i => i.Id).ToList(),
            Timestamp = postDto.Timestamp
        };
    }

    private async Task SendNotifications(TradingForumGrpc.PostDTO updatedPost)
    {
        List<ApiContracts.UserDTO> userDtosByUser = await UserService.GetAllFavoritedBy(updatedPost.Owner);

        if (updatedPost.Categories == null || userDtosByUser == null)
        {
            return;
        }
        
        if (updatedPost.State == 1) //Den er aktiv
        {
            foreach(ApiContracts.UserDTO user in userDtosByUser) //Man følger brugeren
            {
                NotificationService.MakeNotification(new CreateNotificationDTO()
                {
                    User = user.Username, 
                    Message = NewPost(updatedPost.Owner), 
                    NotificationType = 1,
                    ObjectId = updatedPost.Id.ToString()
                });
            }

            foreach (TradingForumGrpc.CategoryDTO category in updatedPost.Categories) //Man følger kategorien
            {
                List<ApiContracts.UserDTO> userDtosByCategory = await UserService.GetUsersFavoritedByCategory(category.Name);
                foreach (ApiContracts.UserDTO user in userDtosByCategory)
                {
                    if (!updatedPost.Owner.Equals(user.Username))
                    {
                        CreateNotificationDTO createNotificationDTO = new CreateNotificationDTO()
                        {
                            User = user.Username,
                            Message = NewPostByCategory(category.Name),
                            NotificationType = 2,
                            ObjectId = updatedPost.Id.ToString()
                        };

                        List<ApiContracts.NotificationDTO> notificationDtos = NotificationService.GetNotifications(user.Username);
                        if (!notificationDtos 
                                .Where(m => m.NotificationType.Equals(createNotificationDTO.NotificationType))
                                .Where(n => n.ObjectId.Equals(createNotificationDTO.ObjectId))
                                .Any()) //Er den oprettet, med en anden kategori?
                        {
                            if (!notificationDtos
                                    .Where(m => m.NotificationType.Equals(1))
                                    .Where(n => n.ObjectId.Equals(createNotificationDTO.ObjectId))
                                    .Any()) //Er den oprettet, hvis allerede man følger brugeren?
                            {
                                NotificationService.MakeNotification(createNotificationDTO); //Sender med én af kategorierne
                            }
                        }
                    }
                }
            }
        }
        else if (updatedPost.State != 1)
        {
            foreach(ApiContracts.UserDTO user in userDtosByUser) //Man følger brugeren
            {
                NotificationService.ReadNotifications(new ReadNotificationsDTO()
                {
                    User = user.Username, 
                    Message = NewPost(updatedPost.Owner), 
                    NotificationType = 1,
                    ObjectId = updatedPost.Id.ToString()
                });
            }
            
            foreach (TradingForumGrpc.CategoryDTO category in updatedPost.Categories) //Man følger kategorien
            {
                List<ApiContracts.UserDTO> userDtosByCategory = await UserService.GetUsersFavoritedByCategory(category.Name);
                foreach (ApiContracts.UserDTO user in userDtosByCategory)
                {
                    NotificationService.ReadNotifications(new ReadNotificationsDTO()
                    {
                        User = user.Username, 
                        Message = NewPostByCategory(category.Name), 
                        NotificationType = 2,
                        ObjectId = updatedPost.Id.ToString()
                    });
                }
            }
        }
    }
    
    private string NewPost(string sender)
    {
        return $"Nyt/ændret opslag fra {sender}";
    }
    
    private string NewPostByCategory(string categoryName)
    {
        return $"Nyt/ændret opslag med {categoryName}";
    }
}