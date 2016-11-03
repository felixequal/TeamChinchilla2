﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MESACCA.Models;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types

namespace MESACCA.DataBaseManagers
{
    public static class BlobManager
    {
        static BlobService blobService = new BlobService();
        //This method takes a Center object and using the filename given adds the image to the BLOB storage and returns the
        //BLOB URI so the image can be accessed from the blob storage for display.
        public static String getCenterImageBLOBURI(Models.Center model)
        {
            String blobURI;
            CloudBlobContainer blobContainer = blobService.GetCloudBlobContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(model.Picture.FileName);
            blob.UploadFromStream(model.Picture.InputStream);
            return blobURI = blob.Uri.ToString();
        }
    }
}