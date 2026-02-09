namespace WroclawTheatreTickets.Application.Contracts.Dtos;

public class UserProfileDto : UserDto
{
    public bool EnableEmailNotifications { get; set; }
    public bool EnablePushNotifications { get; set; }
    public ICollection<string>? PreferredCategories { get; set; }
}
