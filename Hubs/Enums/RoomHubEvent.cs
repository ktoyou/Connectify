namespace GachiHubBackend.Hubs.Enums;

public enum RoomHubEvent
{
    ReceiveMessage,
    JoinedRoom,
    LeavedRoom,
    CreatedRoom,
    JoinedToOtherRoom,
    LeavedFromOtherRoom,
    ScreenShareStarted,
    CameraShareStarted,
    ScreenShareStopped,
    CameraShareStopped,
}