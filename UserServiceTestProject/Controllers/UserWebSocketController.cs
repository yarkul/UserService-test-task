using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using UserServiceTestProject.Services;

namespace UserServiceTestProject.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UserWebSocketController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserWebSocketController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("ws/user")]
        public async Task HandleUserWebSocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                await HandleWebSocket(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        private async Task HandleWebSocket(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                    break;
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    string response;
                    try
                    {
                        var doc = JsonDocument.Parse(message);
                        var root = doc.RootElement;
                        int userId = root.GetProperty("userId").GetInt32();
                        string newRole = root.GetProperty("newRole").GetString();

                        await _userService.UpdateUserRoleAsync(userId, newRole);
                        response = JsonSerializer.Serialize(new { success = true, userId, newRole });
                    }
                    catch (Exception ex)
                    {
                        response = JsonSerializer.Serialize(new { success = false, error = ex.Message });
                    }
                    var responseBuffer = Encoding.UTF8.GetBytes(response);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
} 