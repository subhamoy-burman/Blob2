using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

public class UploadController : Controller
{
    private readonly BlobServiceClient _blobServiceClient;

    public UploadController(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        // Save the file to Azure Blob Storage.
        // Get a reference to the container
        var containerClient = _blobServiceClient.GetBlobContainerClient("timer-blob-objects");

        // Get a reference to the blob
        var blobClient = containerClient.GetBlobClient(file.FileName);

        // Upload the file
        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream);
        }

        // Return a success message.
        return Content("File uploaded successfully.");
    }
}