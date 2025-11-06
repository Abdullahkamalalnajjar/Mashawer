using MediatR;
using Mashawer.Core.Bases;

public class GetUnreadChatsCountQuery : IRequest<Response<int>>
{
    public string UserId { get; set; }
}