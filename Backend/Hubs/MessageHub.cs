using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chatly.Hubs;

[Authorize]
public class MessageHub : Hub
{
}