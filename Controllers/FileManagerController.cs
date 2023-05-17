using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Blob2.Models;
using Azure.Storage.Blobs;

namespace Blob2.Controllers;
public class FileManagerController : Controller
   {
       private readonly BlobServiceClient _blobServiceClient;

       public FileManagerController(BlobServiceClient blobServiceClient)
       {
           _blobServiceClient = blobServiceClient;
       }

       public async Task<IActionResult> Index()
       {
           // Get a list of containers in the storage account
           var containers = _blobServiceClient.GetBlobContainers();

           // Get a list of blobs in each container
           var files = new List<FileModel>();
           foreach (var container in containers)
           {
               var containerClient = _blobServiceClient.GetBlobContainerClient(container.Name);
               var blobs = containerClient.GetBlobs();
               foreach (var blob in blobs)
               {
                   files.Add(new FileModel
                   {
                       FileName = blob.Name,
                       Url = containerClient.GetBlobClient(blob.Name).Uri.AbsoluteUri
                   });
               }
           }

           return View(files);
       }

       public async Task<IActionResult> Download(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("timer-blob-objects");
            var blobClient = containerClient.GetBlobClient(fileName);
            var blobDownloadStreamingResult = await blobClient.DownloadStreamingAsync();
            return File(blobDownloadStreamingResult.Value.Content, blobDownloadStreamingResult.Value.Details.ContentType, fileName);
        }


       public async Task<IActionResult> Delete(string fileName)
       {
           var containerClient = _blobServiceClient.GetBlobContainerClient("timer-blob-objects");
           var blobClient = containerClient.GetBlobClient(fileName);
           await blobClient.DeleteIfExistsAsync();

           return RedirectToAction("Index");
       }

       public async Task<IActionResult> CreateContainer(string containerName)
       {
           var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
           await containerClient.CreateIfNotExistsAsync();

           return RedirectToAction("Index");
       }
   }