namespace Domain.State;

public class ClosedState : IPostState
{
    public ClosedState(Post post, string buyer)
    {
        if (string.IsNullOrEmpty(buyer))
            post.SoldTo =  "";
        else
            post.SoldTo = buyer;
    }
    public void Activate(Post post)
    {
        throw new Exception("Posten er lukket, så du kan ikke aktivere den!");
    }

    public void MakeInactive(Post post)
    {
        throw new Exception("Posten er lukket, så du kan ikke deaktivere den!");
    }

    public void Close(Post post,  string buyer)
    {
        if (string.IsNullOrEmpty(post.SoldTo))
            throw new Exception($"Posten er lukket, så du kan ikke lukke den! Den blev ikke solgt!");
        else
            throw new Exception($"Posten er lukket, så du kan ikke lukke den! Køberen var: {post.SoldTo}");
    }
    
    public PostState GetState(){
        return PostState.CLOSED;
    }
}