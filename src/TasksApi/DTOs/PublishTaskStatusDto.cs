using Core.MessageClient;

namespace TasksApi.DTOs;

public class PublishTaskStatusDto:BasePublishDto
{
    public string Title { get; set; }
    public string OldStatus { get; set; }
    public string NewStatus { get; set; }
}