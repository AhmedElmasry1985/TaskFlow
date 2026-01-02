using Core.MessageClient;

namespace TasksApi.DTOs;

public class PublishNoteDto:BasePublishDto
{
    public string Content { get; set; }
    public string TaskTitle { get; set; }
    public string CreatorUsername { get; set; }
    public DateTime CreationDate { get; set; }
}