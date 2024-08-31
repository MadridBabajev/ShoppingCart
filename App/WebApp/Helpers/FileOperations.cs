namespace Helpers;

public static class FileOperations
{
    public static byte[] LoadImageAsBytes(string imagePath)
    {
        using var image = File.OpenRead(imagePath);
        using var memoryStream = new MemoryStream();
        image.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}