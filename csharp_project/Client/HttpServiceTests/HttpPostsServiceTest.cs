using System.Net.Http.Headers;
using System.Net.Http.Json;
using ApiContracts;
using BlazorApp.Services;

namespace HttpServiceTests;

public class Tests{
    /*
     * These tests assume that the business logic server and database server are running.
     * These test will add entities to the database so you might need to configure the database before and after running.
     * These tests assume there is a user named 'admin'.
     */
    
    private IHttpPostsService postsService; 
    
    [SetUp]
    public async Task Setup(){
        HttpClient httpClient = new HttpClient(){ BaseAddress = new Uri("http://localhost:5012") };
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "auth/login",
            new UserAccessDTO(){Username = "admin", Password = "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg="}
        );
        string jwt = await response.Content.ReadAsStringAsync();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        
        postsService = new HttpPostsService(httpClient);
    }

    private CreatePostDTO GetBaseCreatePostDTO(){
        return new CreatePostDTO(){
            Title = "MyTitle", 
            Creator = "admin", 
            Categories = [], 
            Description = "MyDescription", 
            Images = [], 
            Price = 10.0, 
            WantsToTrade = false, 
            ZipCode = "8700"
        };
    }

    [Test]
    public void MakePost(){
        CreatePostDTO request = GetBaseCreatePostDTO();

        PostDTO response = null;
        Assert.DoesNotThrowAsync(async () => {
            response = await postsService.MakePostAsync(request);
        });

        Assert.That(response.Id > 0);
        Assert.That(response.Title == "MyTitle");
        Assert.That(response.Description == "MyDescription");
        Assert.That(response.Price == 10.0);
        Assert.That(response.ZipCode == "8700");
        Assert.That(response.WantsToTrade == false);
        Assert.That(response.SoldTo == "");
        Assert.That(response.State == 0); // inactive
        Assert.That(response.Owner == "admin");
        Assert.That(response.Categories.Count == 0);
        Assert.That(response.ImageIds.Count == 0);
        
        Assert.That(DateTime.TryParse(response.Timestamp, out DateTime time));
        Assert.That(DateTime.Now.Subtract(TimeSpan.FromSeconds(10)) < time && time < DateTime.Now);
    }
    
    [Test]
    public void GetPost(){
        CreatePostDTO request = GetBaseCreatePostDTO();

        PostDTO response = null;
        Assert.DoesNotThrowAsync(async () => {
            response = await postsService.MakePostAsync(request);
            response = await postsService.GetPost(response.Id);
        });

        Assert.That(response.Id > 0);
        Assert.That(response.Title == "MyTitle");
        Assert.That(response.Description == "MyDescription");
        Assert.That(response.Price == 10.0);
        Assert.That(response.ZipCode == "8700");
        Assert.That(response.WantsToTrade == false);
        Assert.That(response.SoldTo == "");
        Assert.That(response.State == 0); // inactive
        Assert.That(response.Owner == "admin");
        Assert.That(response.Categories.Count == 0);
        Assert.That(response.ImageIds.Count == 0);
        
        Assert.That(DateTime.TryParse(response.Timestamp, out DateTime time));
        Assert.That(DateTime.Now.Subtract(TimeSpan.FromSeconds(10)) < time && time < DateTime.Now);
    }

    [Test]
    public void GetPosts(){
        CreatePostDTO request = GetBaseCreatePostDTO();

        PostDTO postDto = null;
        List<PostDTO> response = null;
        Assert.DoesNotThrowAsync(async () => {
            postDto = await postsService.MakePostAsync(request);
            response = await postsService.GetPosts();
        });

        Assert.That(response.Count > 0);
        Assert.That(response.Any(p => p.Id == postDto.Id));
        postDto = response.First(p => p.Id == postDto.Id);
        
        Assert.That(postDto.Id > 0);
        Assert.That(postDto.Title == "MyTitle");
        Assert.That(postDto.Description == "MyDescription");
        Assert.That(postDto.Price == 10.0);
        Assert.That(postDto.ZipCode == "8700");
        Assert.That(postDto.WantsToTrade == false);
        Assert.That(postDto.SoldTo == "");
        Assert.That(postDto.State == 0); // inactive
        Assert.That(postDto.Owner == "admin");
        Assert.That(postDto.Categories.Count == 0);
        Assert.That(postDto.ImageIds.Count == 0);
    }
    
    [Test]
    public void MakePostWithCategories(){
        CreatePostDTO request = GetBaseCreatePostDTO();
        request.Categories = [new CategoryDTO(){ Name = "Elektronik" }, new CategoryDTO(){Name = "Flyvemaskiner"}];

        PostDTO response = null;
        Assert.DoesNotThrowAsync(async () => {
            response = await postsService.MakePostAsync(request);
        });
        
        Assert.That(response.Categories.Count == 2);
        Assert.That(response.Categories.Any(c => c.Name == "Elektronik"));
        Assert.That(response.Categories.Any(c => c.Name == "Flyvemaskiner"));
    }
    
    [Test]
    public void MakePostWithImages(){
        CreatePostDTO request = GetBaseCreatePostDTO();
        request.Images = ["iVBORw0KGgoAAAANSUhEUgAAABAAAAAJCAYAAAA7KqwyAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABQSURBVChTY/gPBW9lVMAYGSht9EHh23ucwWAzgAiYRooMwKaZKAPw2Y5sADbNIAA2gBgXUM0AZM0ggNUAmEYYjct2ECDJAHTNIIA1EEkxAAAP2tocexQJBQAAAABJRU5ErkJggg=="];

        PostDTO response = null;
        Assert.DoesNotThrowAsync(async () => {
            response = await postsService.MakePostAsync(request);
        });
        
        Assert.That(response.ImageIds.Count == 1);
    }
    
    [Test]
    public void MakePostWithNonImage(){
        CreatePostDTO request = GetBaseCreatePostDTO();
        request.Images = ["aGVsbG8gdGVzdA=="];

        PostDTO response = null;
        Assert.ThrowsAsync<Exception>(async () => {
            response = await postsService.MakePostAsync(request);
        });
    }
    
    [Test]
    public void MakePostWithNoTitle(){
        CreatePostDTO request = GetBaseCreatePostDTO();
        request.Title = "";

        PostDTO response = null;
        Assert.ThrowsAsync<Exception>(async () => {
            response = await postsService.MakePostAsync(request);
        });
    }
    
    [Test]
    public void MakePostWithNoDescription(){
        CreatePostDTO request = GetBaseCreatePostDTO();
        request.Description = "";

        PostDTO response = null;
        Assert.ThrowsAsync<Exception>(async () => {
            response = await postsService.MakePostAsync(request);
        });
    }
    
    [Test]
    public void MakePostWithNoZipCode(){
        CreatePostDTO request = GetBaseCreatePostDTO();
        request.ZipCode = "";

        PostDTO response = null;
        Assert.ThrowsAsync<Exception>(async () => {
            response = await postsService.MakePostAsync(request);
        });
    }
    
    [Test]
    public void MakePostWithInvalidZipCode(){
        CreatePostDTO request = GetBaseCreatePostDTO();
        request.ZipCode = "MyZipCode";

        PostDTO response = null;
        Assert.ThrowsAsync<Exception>(async () => {
            response = await postsService.MakePostAsync(request);
        });
    }
    
    [Test]
    public void MakePostWithInvalidCreator(){
        CreatePostDTO request = GetBaseCreatePostDTO();
        request.Creator = "Darth";

        PostDTO response = null;
        Assert.ThrowsAsync<Exception>(async () => {
            response = await postsService.MakePostAsync(request);
        });
    }
    
    [Test]
    public void ChangePost(){
        CreatePostDTO request = GetBaseCreatePostDTO();

        PostDTO response = null;
        Assert.DoesNotThrowAsync(async () => {
            response = await postsService.MakePostAsync(request);
        });
        
        response.Title = "MyNewTitle";
        response.Description = "MyNewDescription";
        
        Assert.DoesNotThrowAsync(async () => {
            await postsService.ChangePostAsync(response, response.Id);
            response = await postsService.GetPost(response.Id);
        });
        
        Assert.That(response.Title == "MyNewTitle");
        Assert.That(response.Description == "MyNewDescription");
    }
    
    [Test]
    public void ChangePostToActive(){
        CreatePostDTO request = GetBaseCreatePostDTO();

        PostDTO response = null;
        Assert.DoesNotThrowAsync(async () => {
            response = await postsService.MakePostAsync(request);
            await postsService.ChangePostStateAsync(new ChangePostStateDTO(){makeActive = true, postId = response.Id});
            response = await postsService.GetPost(response.Id);
        });
        
        Assert.That(response.State == 1);
    }
    
    [Test]
    public void ChangePostToInactive(){
        CreatePostDTO request = GetBaseCreatePostDTO();

        PostDTO response = null;
        Assert.DoesNotThrowAsync(async () => {
            response = await postsService.MakePostAsync(request);
            await postsService.ChangePostStateAsync(new ChangePostStateDTO(){makeActive = true, postId = response.Id});
            await postsService.ChangePostStateAsync(new ChangePostStateDTO(){makeActive = false, postId = response.Id});
            response = await postsService.GetPost(response.Id);
        });
        
        Assert.That(response.State == 0);
    }
    
    [Test]
    public void ChangePostToActiveWhenActive(){
        CreatePostDTO request = GetBaseCreatePostDTO();

        PostDTO response = null;
        Assert.DoesNotThrowAsync(async () => {
            response = await postsService.MakePostAsync(request);
            await postsService.ChangePostStateAsync(new ChangePostStateDTO(){makeActive = true, postId = response.Id});
        });
        
        Assert.ThrowsAsync<Exception>(async () => {
            await postsService.ChangePostStateAsync(new ChangePostStateDTO(){makeActive = true, postId = response.Id});
        });
    }
    
    [Test]
    public void ChangePostToInactiveWhenInactive(){
        CreatePostDTO request = GetBaseCreatePostDTO();

        PostDTO response = null;
        Assert.ThrowsAsync<Exception>(async () => {
            response = await postsService.MakePostAsync(request);
            await postsService.ChangePostStateAsync(new ChangePostStateDTO(){makeActive = false, postId = response.Id});
        });
    }
    
    [Test]
    public void ClosePost(){
        CreatePostDTO request = GetBaseCreatePostDTO();

        PostDTO response = null;
        Assert.DoesNotThrowAsync(async () => {
            response = await postsService.MakePostAsync(request);
            await postsService.ClosePostAsync(new ClosePostDTO(){ isSold = false, postId = response.Id, soldTo = null });
            response = await postsService.GetPost(response.Id);
        });
        
        Assert.That(response.State == 2);
        Assert.That(response.SoldTo == "");
    }
    
    [Test]
    public void TryChangePostAfterClose(){
        CreatePostDTO request = GetBaseCreatePostDTO();

        PostDTO response = null;
        Assert.ThrowsAsync<Exception>(async () => {
            response = await postsService.MakePostAsync(request);
            await postsService.ClosePostAsync(new ClosePostDTO(){ isSold = false, postId = response.Id, soldTo = null });
            response.Title = "MyNewTitle";
            await postsService.ChangePostAsync(response, response.Id);
        });
    }
    
    [Test]
    public void SellPost(){
        CreatePostDTO request = GetBaseCreatePostDTO();

        PostDTO response = null;
        Assert.DoesNotThrowAsync(async () => {
            response = await postsService.MakePostAsync(request);
            await postsService.ClosePostAsync(new ClosePostDTO(){ isSold = true, postId = response.Id, soldTo = "bob" });
            response = await postsService.GetPost(response.Id);
        });
        
        Assert.That(response.State == 2);
        Assert.That(response.SoldTo == "bob");
    }
}