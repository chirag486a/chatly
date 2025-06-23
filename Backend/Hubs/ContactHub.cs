using Chatly.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chatly.Hubs;

[Authorize]
public class ContactHub : Hub
{
}