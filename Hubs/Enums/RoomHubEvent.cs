namespace GachiHubBackend.Hubs.Enums;

public enum RoomHubEvent
{
    ReceiveOffer,
    ReceiveAnswer,
    ReceiveCandidate,
    ReceiveMessage,
    JoinedRoom,
    LeavedRoom,
    CreatedRoom,
    JoinedToOtherRoom,
    LeavedFromOtherRoom,
    ScreenShareStarted,
    CameraShareStarted,
    ScreenShareStopped,
    CameraShareStopped
}