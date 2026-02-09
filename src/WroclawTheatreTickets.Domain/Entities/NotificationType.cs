namespace WroclawTheatreTickets.Domain.Entities;

public enum NotificationType
{
    EventReminder,          // Reminder about upcoming event
    NewEventInCategory,     // New event in favorite category
    ReviewResponse,         // Admin response to review
    SystemAlert,            // System-wide announcements
    WeeklyDigest            // Weekly newsletter
}
