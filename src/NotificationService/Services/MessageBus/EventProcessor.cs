using System.Text.Json;
using Core;
using NotificationService.DTOs;

namespace NotificationService.Services.MessageBus;

public class EventProcessor:IEventProcessor
{
    private const string
        UnknownEventMsg = "Unknown event: '{0}'.",
        RegisteredUserEvent = "User '{0}' with username '{1}' registered.",
        TaskCreatedEvent = "Task '{0}' created by'{1}' and assigned to '{2}'.",
        NoteAddedEvent = "Note '{0}' added to task '{1}' by '{2}'.",
        TaskStatusChangedEvent = "Task '{0}' status changed from '{1}' to '{2}'.";

    public Task<bool> ProcessEvent(string message)
    {
        var messageDto = JsonSerializer.Deserialize<MessageDto>(message);
        Console.Write($"--> {DateTime.UtcNow:HH:mm:ss}: ");
        switch (messageDto?.EventName)
        {
            case Strings.UserRegisteredEvent:
                var registeredUserMsg = JsonSerializer.Deserialize<RegisterUserMessageDto>(message);
                Console.WriteLine(RegisteredUserEvent, registeredUserMsg?.FullName, registeredUserMsg?.Username);
                break;
            case Strings.TaskCreatedEvent:
                var taskCreatedMsg = JsonSerializer.Deserialize<TaskCreatedMessageDto>(message);
                Console.WriteLine(TaskCreatedEvent, taskCreatedMsg?.Title, taskCreatedMsg?.CreatorUsername, taskCreatedMsg?.AssignedUsername);
                break;
            case Strings.NoteAddedEvent:
                var noteAddedMsg = JsonSerializer.Deserialize<PublishNoteMessageDto>(message);
                Console.WriteLine(NoteAddedEvent, noteAddedMsg?.Content, noteAddedMsg?.TaskTitle, noteAddedMsg?.CreatorUsername);
                break;
            case Strings.TaskStatusChangedEvent:
                var taskStatusChangedMsg = JsonSerializer.Deserialize<PublishTaskStatusMessageDto>(message);
                Console.WriteLine(TaskStatusChangedEvent, taskStatusChangedMsg?.Title, taskStatusChangedMsg?.OldStatus, taskStatusChangedMsg?.NewStatus);
                break;
            default:
                Console.WriteLine(UnknownEventMsg, messageDto?.EventName);
                break;
        }
        return Task.FromResult(true);
    }
}