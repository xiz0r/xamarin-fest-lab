using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared;
using Plugin.Media.Abstractions;

namespace Cognitive.Services.Lab
{
	public static class AzureStorage
	{
		public static CloudBlobContainer GetContainer(string containerType)
		{
			var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=imgfest;AccountKey=nmVQ9t5Pv/zjoaUHLz3ABzJkvJSHY17Fbf7KJiWUOUBUxYOq5fIQwhHBOyavtaWZxrxnqkMsqiN8XqL4jU0vfw==;EndpointSuffix=core.windows.net");
			var client = account.CreateCloudBlobClient();
			return client.GetContainerReference(containerType.ToLower());
		}

		public static async Task<IEnumerable<ImageModel>> GetBlobImages()
		{
			try
			{
				var container = GetContainer("event");

				var allBlobsList = new List<ImageModel>();
				BlobContinuationToken token = null;

				do
				{
					var result = await container.ListBlobsSegmentedAsync(token);
					if (result.Results.Count() > 0)
					{
						var blobs = result.Results.Cast<CloudBlockBlob>()
										  .Select(b => new ImageModel
										  {
											  Name = b.Name,
											  StorageUri = b.StorageUri.PrimaryUri.ToString()
										  });
						allBlobsList.AddRange(blobs);
					}
					token = result.ContinuationToken;
				} while (token != null);

				return allBlobsList;
			}
			catch
			{
				return Enumerable.Empty<ImageModel>();
			}
		}

		public static async Task<ImageModel> UploadImage(MediaFile file)
		{
			var container = GetContainer("event");
			await container.CreateIfNotExistsAsync();

			var name = $"XamarinFest-{Guid.NewGuid()}";
			var fileBlob = container.GetBlockBlobReference(name);
			await fileBlob.UploadFromStreamAsync(file.GetStream());

			return new ImageModel
			{
				Name = name,
				StorageUri = fileBlob.StorageUri.PrimaryUri.ToString()
			};
		}
	}
}