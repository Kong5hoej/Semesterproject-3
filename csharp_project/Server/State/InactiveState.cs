namespace Domain.State;

public class InactiveState : IPostState
{
    public void Activate(Post post)
    {
        post.State = new ActiveState();
    }

    public void MakeInactive(Post post)
    {
        throw new Exception("Postet er allerede inaktivt!");
    }

    public void Close(Post post, string buyer)
    {
        post.State = new ClosedState(post, buyer);
    }

    public PostState GetState(){
        return PostState.INACTIVE;
    }
}