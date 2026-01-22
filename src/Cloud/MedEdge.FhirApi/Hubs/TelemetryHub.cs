using Microsoft.AspNetCore.SignalR;

namespace MedEdge.FhirApi.Hubs;

/// <summary>
/// SignalR Hub for real-time telemetry streaming to connected dashboard clients
/// </summary>
public class TelemetryHub : Hub
{
    private static readonly Dictionary<string, HashSet<string>> DeviceSubscriptions = new();
    private readonly ILogger<TelemetryHub> _logger;

    public TelemetryHub(ILogger<TelemetryHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");

        // Clean up subscriptions
        foreach (var device in DeviceSubscriptions.Keys.ToList())
        {
            if (DeviceSubscriptions[device].Contains(Context.ConnectionId))
            {
                DeviceSubscriptions[device].Remove(Context.ConnectionId);
                if (DeviceSubscriptions[device].Count == 0)
                {
                    DeviceSubscriptions.Remove(device);
                }
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Subscribe to real-time telemetry updates for a specific device
    /// </summary>
    /// <param name="deviceId">Device identifier to subscribe to</param>
    public async Task SubscribeToDevice(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            await Clients.Caller.SendAsync("Error", "Device ID cannot be empty");
            return;
        }

        if (!DeviceSubscriptions.ContainsKey(deviceId))
        {
            DeviceSubscriptions[deviceId] = new HashSet<string>();
        }

        if (DeviceSubscriptions[deviceId].Add(Context.ConnectionId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);
            _logger.LogInformation($"Client {Context.ConnectionId} subscribed to device {deviceId}");
            await Clients.Caller.SendAsync("SubscriptionConfirmed", new { deviceId });
        }
    }

    /// <summary>
    /// Unsubscribe from real-time telemetry updates for a specific device
    /// </summary>
    /// <param name="deviceId">Device identifier to unsubscribe from</param>
    public async Task UnsubscribeFromDevice(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            return;

        if (DeviceSubscriptions.ContainsKey(deviceId))
        {
            DeviceSubscriptions[deviceId].Remove(Context.ConnectionId);
            if (DeviceSubscriptions[deviceId].Count == 0)
            {
                DeviceSubscriptions.Remove(deviceId);
            }
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, deviceId);
        _logger.LogInformation($"Client {Context.ConnectionId} unsubscribed from device {deviceId}");
    }

    /// <summary>
    /// Broadcast a vital sign update to all subscribers of a device
    /// </summary>
    public async Task BroadcastVitalSignUpdate(string deviceId, object vitalSignUpdate)
    {
        await Clients.Group(deviceId).SendAsync("VitalSignUpdate", vitalSignUpdate);
    }

    /// <summary>
    /// Broadcast clinical alerts to all subscribers of a device
    /// </summary>
    public async Task BroadcastAlerts(string deviceId, object alerts)
    {
        await Clients.Group(deviceId).SendAsync("AlertsReceived", alerts);
    }

    /// <summary>
    /// Get list of all devices with active subscriptions
    /// </summary>
    public async Task<IEnumerable<string>> GetActiveDevices()
    {
        return await Task.FromResult(DeviceSubscriptions.Keys);
    }

    /// <summary>
    /// Get subscription count for a specific device
    /// </summary>
    public async Task<int> GetSubscriberCount(string deviceId)
    {
        return await Task.FromResult(DeviceSubscriptions.ContainsKey(deviceId) ? DeviceSubscriptions[deviceId].Count : 0);
    }
}
