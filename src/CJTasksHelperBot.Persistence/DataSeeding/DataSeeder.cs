using Bogus;
using CJTasksHelperBot.Domain.Entities;
using CJTasksHelperBot.Persistence.DataSeeding.DataContainers;
using CJTasksHelperBot.Persistence.DataSeeding.Stages;
using Task = CJTasksHelperBot.Domain.Entities.Task;

namespace CJTasksHelperBot.Persistence.DataSeeding;

public class DataSeeder :
    ICanAddEntity
{
    public readonly EntityList<User> Users;
    public readonly EntityList<Chat> Chats;
    public readonly EntityList<UserChat> UserChats;
    public readonly EntityList<Task> Tasks;
    public readonly EntityList<UserTaskStatus> UserTaskStatuses;
    private readonly ApplicationDbContext _context;
    
    private DataSeeder(ApplicationDbContext context)
    {
        _context = context;
        Users = new EntityList<User>((x, y) => x.Id == y.Id);
        UserChats = new EntityList<UserChat>((x, y) => x.UserId == y.UserId && x.ChatId == y.ChatId);
        Chats = new EntityList<Chat>((x, y) => x.Id == y.Id);
        Tasks = new EntityList<Task>((x, y) => x.Id == y.Id);
        UserTaskStatuses = new EntityList<UserTaskStatus>((x, y) => x.UserId == y.UserId && x.TaskId == y.TaskId);
    }

    public static ICanAddEntity CreateSeeder(ApplicationDbContext context)
    {
        return new DataSeeder(context);
    }

    public async System.Threading.Tasks.Task SeedAsync()
    {
        _context.Users.AddRange(Users);
        _context.Chats.AddRange(Chats);
        _context.UserChats.AddRange(UserChats);
        _context.Tasks.AddRange(Tasks);
        _context.UserTaskStatuses.AddRange(UserTaskStatuses);
        await _context.SaveChangesAsync();
    }

    public ICanAddEntity AddUsers(long chatId, params long[] userIds)
    {
        if (userIds.Length == 0) return this;
        
        if (Chats.FirstOrDefault(x => x.Id == chatId) == null)
            AddChats(chatId);
        
        var userFaker = new Faker<User>()
            .RuleFor(x => x.Id, f => userIds[f.IndexFaker])
            .RuleFor(x => x.FirstName, f => f.Person.FirstName)
            .RuleFor(x => x.LastName, f => f.Person.LastName)
            .RuleFor(x => x.UserName, f => f.Person.UserName);
        
        var users = userFaker.Generate(userIds.Length);

        var userChatFaker = new Faker<UserChat>()
            .RuleFor(x => x.UserId, f => userIds[f.IndexFaker])
            .RuleFor(x => x.ChatId, chatId);

        var userChats = userChatFaker.Generate(userIds.Length);
        
        Users.AddIfNotExistsRange(users);
        UserChats.AddIfNotExistsRange(userChats);
        
        return this;
    }

    public IUserDataContainer AddUser(long chatId = 0, long userId = 0)
    {
        if (Chats.FirstOrDefault(x => x.Id == chatId) == null)
            AddChats(chatId);

        var user = Users.FirstOrDefault(x => x.Id == userId);
        var userChat = UserChats.FirstOrDefault(x => x.UserId == userId && x.ChatId == chatId);
        if (user != null && userChat != null)
        {
            return new UserDataContainer(this, user, userChat);
        }
        
        var userFaker = new Faker<User>()
            .RuleFor(x => x.Id, userId)
            .RuleFor(x => x.FirstName, f => f.Person.FirstName)
            .RuleFor(x => x.LastName, f => f.Person.LastName)
            .RuleFor(x => x.UserName, f => f.Person.UserName);

        user = userFaker.Generate();
        
        var userChatFaker = new Faker<UserChat>()
            .RuleFor(x => x.UserId, userId)
            .RuleFor(x => x.ChatId, chatId);

        userChat = userChatFaker.Generate();

        return new UserDataContainer(this, user, userChat);
    }

    public ICanAddEntity AddChats(params long[] chatIds)
    {
        if (chatIds.Length == 0) return this;
        
        var faker = new Faker<Chat>()
            .RuleFor(x => x.Id, f => chatIds[f.IndexFaker])
            .RuleFor(x => x.Title, f => f.Lorem.Slug());
        
        var chats = faker.Generate(chatIds.Length);

        Chats.AddIfNotExistsRange(chats);
        
        return this;
    }
}