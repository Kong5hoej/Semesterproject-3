namespace Domain.State;

public interface IPostState
{
    public void Activate(Post post);
    public void MakeInactive(Post post);
    public void Close(Post post, string buyer);
    public PostState GetState();
}