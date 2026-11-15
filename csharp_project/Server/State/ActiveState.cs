namespace Domain.State;

public class ActiveState : IPostState
{
    public void Activate(Post post)
    {
        throw new Exception("Posten er allerede aktivt!");
    }

    public void MakeInactive(Post post)
    {
        post.State = new InactiveState();
    }

    public void Close(Post post, string buyer)
    {
        post.State = new ClosedState(post, buyer);
    }
    
    public PostState GetState(){
        return PostState.ACTIVE;
    }
}