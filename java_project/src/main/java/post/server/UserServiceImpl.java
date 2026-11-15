package post.server;

import io.grpc.stub.StreamObserver;
import post.database.CategoryDao;
import post.database.UserDao;
import post.domain.Category;
import post.domain.User;
import tradingForumGrpc.*;

import java.util.List;
import java.util.Set;

public class UserServiceImpl extends UserServiceGrpc.UserServiceImplBase {
    private final UserDao userDAO;
    private final CategoryDao categoryDAO;

    public UserServiceImpl(UserDao userDAO, CategoryDao categoryDAO) {
        this.userDAO = userDAO;
        this.categoryDAO = categoryDAO;
    }

    @Override
    public void createUser(CreateUserRequest request,
                           StreamObserver<UserDTO> responseObserver) {
        User user = new User(request.getUsername(), request.getPassword(),
                request.getZipCode());

        user = userDAO.createUser(user);
        UserDTO response = userDTOFromUser(user);

        responseObserver.onNext(response);
        responseObserver.onCompleted();
    }

    @Override
    public void login(LoginRequest request,
                      StreamObserver<LoginResponse> responseObserver) {
        String password = userDAO.getHashedPassword(request.getUsername());
        LoginResponse response = LoginResponse.newBuilder().setPassword(password)
                .build();

        responseObserver.onNext(response);
        responseObserver.onCompleted();
    }

    @Override
    public void getUser(GetUserRequest request,
                        StreamObserver<UserDTO> responseObserver) {
        User user = userDAO.getUserById(request.getUsername());
        UserDTO response = userDTOFromUser(user);

        responseObserver.onNext(response);
        responseObserver.onCompleted();
    }

    @Override
    public void getAllUsers(Empty request,
                            StreamObserver<GetUsersResponse> responseObserver) {
        List<User> users = userDAO.getAllUsers();

        List<UserDTO> userDTOs = users.stream().map(this::userDTOFromUser).toList();

        GetUsersResponse response = GetUsersResponse.newBuilder()
                .addAllUser(userDTOs).build();

        responseObserver.onNext(response);
        responseObserver.onCompleted();
    }

    @Override
    public void getFavorites(tradingForumGrpc.UserDTO request,
                             io.grpc.stub.StreamObserver<tradingForumGrpc.GetFavoritesResponse> responseObserver) {
        User user = userDAO.getUserById(request.getUsername());

        GetFavoritesResponse response = GetFavoritesResponse.newBuilder()
                .addAllIds(
                        user.getFavorites().stream().map(p -> p.getId())
                                .toList())
                .build();

        responseObserver.onNext(response);
        responseObserver.onCompleted();
    }

    @Override
    public void changeFavorite(tradingForumGrpc.ChangeFavoriteDTO request,
                               io.grpc.stub.StreamObserver<tradingForumGrpc.Empty> responseObserver) {
        userDAO.changeFavorite(request.getUser(), request.getPostId(), request.getFavorite());

        responseObserver.onNext(Empty.newBuilder().build());
        responseObserver.onCompleted();
    }

    @Override
    public void changeFavoriteUser(tradingForumGrpc.ChangeFavoriteUserDTO request,
                                   io.grpc.stub.StreamObserver<tradingForumGrpc.Empty> responseObserver) {
    userDAO.changeFavoriteUser(request.getUsername(), request.getFavoriteUsername(), request.getFavorite());

    responseObserver.onNext(Empty.newBuilder().build());
    responseObserver.onCompleted();
    }

    @Override
    public void getFavoriteUsers(tradingForumGrpc.GetFavoriteUsersRequest request,
                                 io.grpc.stub.StreamObserver<tradingForumGrpc.GetFavoriteUsersResponse> responseObserver) {
        User user = userDAO.getUserById(request.getUsername());

        GetFavoriteUsersResponse response = GetFavoriteUsersResponse.newBuilder()
                .addAllFavoriteUsernames(user.getFavoriteUsers().stream()
                        .map(u -> u.getUsername()).toList())
                .build();

        responseObserver.onNext(response);
        responseObserver.onCompleted();
    }

    @Override
    public void getFavoritedBy(tradingForumGrpc.GetFavoritedByUsersRequest request, io.grpc.stub.StreamObserver<tradingForumGrpc.GetUsersResponse> responseObserver)
    {
        List<User> users = userDAO.getAllFavoritedBy(request.getUsername());

        List<UserDTO> userDTOs = users.stream().map(this::userDTOFromUser).toList();

        GetUsersResponse response = GetUsersResponse.newBuilder()
            .addAllUser(userDTOs).build();

        responseObserver.onNext(response);
        responseObserver.onCompleted();
    }

    @Override
    public void changeFavoriteCategory(ChangeFavoriteCategoryRequest request, StreamObserver<Empty> responseObserver)
    {
        userDAO.changeFavoriteCategory(request.getUsername(), request.getCategory());
        responseObserver.onNext(Empty.newBuilder().build());
        responseObserver.onCompleted();
    }

    @Override
    public void getFavoriteCategories(GetFavoriteCategoriesRequest request, StreamObserver<GetFavoriteCategoriesResponse> responseObserver)
    {
        List<Category> categories = userDAO.getAllFavoritedCategories(request.getUsername());

        GetFavoriteCategoriesResponse response = GetFavoriteCategoriesResponse.newBuilder()
            .addAllCategories(categories.stream().map(this::categoryDTOFromCategory).toList())
            .build();

        responseObserver.onNext(GetFavoriteCategoriesResponse.newBuilder().addAllCategories(
            response.getCategoriesList())
            .build());
        responseObserver.onCompleted();
    }

    @Override
    public void deleteAllFavoritedCategories(DeleteFavoriteCategoriesRequest request, StreamObserver<Empty> responseObserver)
    {
        userDAO.deleteAllFavoritedCategories(request.getUsername());
        responseObserver.onNext(Empty.newBuilder().build());
        responseObserver.onCompleted();
    }

    @Override
    public void getUsersFavoritedByCategory(GetUsersFavoritedByCategoryRequest request, StreamObserver<GetUsersFavoritedByCategoryResponse> responseObserver)
    {
        Set<User> users = categoryDAO.getCategoryById(request.getCategoryName()).getUsers();

        List<UserDTO> userDTOs = users.stream().map(this::userDTOFromUser).toList();

        GetUsersFavoritedByCategoryResponse response = GetUsersFavoritedByCategoryResponse.newBuilder()
            .addAllUsers(userDTOs)
            .build();

        responseObserver.onNext(GetUsersFavoritedByCategoryResponse.newBuilder().addAllUsers(
                response.getUsersList())
            .build());
        responseObserver.onCompleted();

    }

    private CategoryDTO categoryDTOFromCategory(Category category)
    {
        CategoryDTO categoryDTO = CategoryDTO.newBuilder()
            .setName(category.getName())
            .build();
        return categoryDTO;
    }

    private UserDTO userDTOFromUser(User user) {
        UserDTO userDTO = UserDTO.newBuilder().setUsername(user.getUsername())
            .setZipCode(user.getZipCode()).build();

        return userDTO;
    }

    public void editUser(UserDTO request,
        StreamObserver<UserDTO> responseObserver) {
        User user = new User();
        user.setUsername(request.getUsername());
        user.setZipCode(request.getZipCode());

        userDAO.editUser(request.getUsername(), user);
        responseObserver.onNext(userDTOFromUser(user));
        responseObserver.onCompleted();
    }
}
