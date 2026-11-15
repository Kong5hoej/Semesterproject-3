package post.domain;

public enum NotificationType
{
  MESSAGE(0),
  LISTING_FROM_USER(1),
  LISTING_FROM_CATEGORY(2);

  private final int type;

  NotificationType(int type) {
    this.type = type;
  }

  public int getValue() {
    return type;
  }

  static public NotificationType getState(int type)
  {
    return switch(type)
    {
      case 0 -> MESSAGE;
      case 1 -> LISTING_FROM_USER;
      case 2 -> LISTING_FROM_CATEGORY;
      default -> MESSAGE;
    };
  }
}
