namespace Eclipse.WebAPI.Extensions;

public static class FormFileExtensions
{
    public static async Task<byte[]> ReadAllBytesAsync(this IFormFile formFile, CancellationToken cancellationToken = default)
    {
        using var stream = await formFile.OpenMemoryStreamAsync(cancellationToken);

        return stream.ToArray();
    }

    public static async Task<MemoryStream> OpenMemoryStreamAsync(this IFormFile formFile, CancellationToken cancellationToken = default)
    {
        using var stream = formFile.OpenReadStream();

        if (stream is MemoryStream memoryStream)
        {
            return memoryStream;
        }

        return await stream.CreateMemoryStreamAsync(cancellationToken);
    }

    public static async Task<MemoryStream> CreateMemoryStreamAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream, cancellationToken);

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        memoryStream.Position = 0;

        return memoryStream;
    }

    public static MemoryStream CreateMemoryStream(this Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        var memoryStream = new MemoryStream();

        stream.CopyTo(memoryStream);

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        memoryStream.Position = 0;

        return memoryStream;
    }
}
