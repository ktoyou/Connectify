namespace GachiHubBackend.Services;

public class AvatarService
{
    public async Task<string> UploadAvatarAsync(IFormFile avatar)
    {
        var uploadFolder = Path.Combine("/etc/vibecast/", "wwwroot", "avatars");
        Directory.CreateDirectory(uploadFolder);

        var fileName = Guid.NewGuid() + Path.GetExtension(avatar.FileName);
        var filePath = Path.Combine(uploadFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await avatar.CopyToAsync(stream);

        return fileName;
    }
}