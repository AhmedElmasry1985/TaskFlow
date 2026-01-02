using Core.MessageClient;

namespace TasksApi.DTOs;

public class PublishTaskDto:BasePublishDto
{
    public string Title { get; set; }
    public string CreatorUsername { get; set; }
    public string AssignedUsername { get; set; }
    public DateTime CreationDate { get; set; }
}