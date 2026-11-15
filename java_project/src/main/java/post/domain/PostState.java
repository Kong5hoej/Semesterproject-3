package post.domain;

public enum PostState{
  INACTIVE(0),
  ACTIVE(1),
  CLOSED(2);

  private final int id;
  PostState(int id) { this.id = id; }
  public int getValue() { return id; }
  public static PostState getState(int id){
    return switch(id){
      case 1 -> ACTIVE;
      case 2 -> CLOSED;
      default -> INACTIVE;
    };
  }
}
