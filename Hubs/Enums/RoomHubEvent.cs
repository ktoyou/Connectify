namespace GachiHubBackend.Hubs.Enums;

public enum RoomHubEvent
{
    ReceiveOffer,
    ReceiveAnswer,
    ReceiveCandidate,
    ReceiveMessage,
    JoinedRoom,
    LeavedRoom,
    LeftedRoom,
    CreatedRoom,
    JoinedToOtherRoom,
    LeavedFromOtherRoom,
    InitiateOffer,
    ScreenShareStarted,
    CameraShareStarted,
    ScreenShareStopped,
    CameraShareStopped
}