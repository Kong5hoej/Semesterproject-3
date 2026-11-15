using System.Runtime.InteropServices.Swift;
using Domain.State;

namespace Domain;

public class Post
{
    public string Title { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public string ZipCode { get; set; }
    public bool WantsToTrade { get; set; }
    public string SoldTo { get; set; }
    public IPostState State { get; set; }
    public string Timestamp { get; set; }

    public Post(string title, string description, double price, string zipCode, bool wantsToTrade, string Timestamp)
    {
        this.Title = title;
        this.Description = description;
        this.Price = price;
        this.ZipCode = zipCode;
        this.WantsToTrade = wantsToTrade;
        this.SoldTo = "";
        this.State = new InactiveState();
        this.Timestamp = Timestamp;
    }

    public void SetState(PostState newState){
        State = newState switch{
            PostState.INACTIVE => new InactiveState(),
            PostState.ACTIVE => new ActiveState(),
            PostState.CLOSED => new ClosedState(this, SoldTo),
            _ => State
        };
    }

    public PostState GetState(){
        return State.GetState();
    }

    public bool IsActive()
    {
        if(State is ActiveState)
            return true;
        return false;
    }
    
    public bool IsClosed()
    {
        if(State is ClosedState)
            return true;
        return false;
    }
    
    public void Activate()
    {
        State.Activate(this);
    }

    public void MakeInactive()
    {
        State.MakeInactive(this);
    }

    public void Close(string buyer)
    {
        State.Close(this, buyer);
    }
    public void format()
    {
        Console.WriteLine($"Title: {Title} \n" +
                          $"ZipCode: {ZipCode} \n" +
                          $"Price: {Price} \n" +
                          $"Wants to trade: {WantsToTrade} \n" +
                          $"\n" +
                          $"{Description}");
    }
}