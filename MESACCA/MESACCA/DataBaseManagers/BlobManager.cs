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
        private static BlobService blobService = new BlobService();

        //Uploads a given file into BLOB storage and returns the URI where it is located.
        public static String uploadAndGetImageBLOBURI(HttpPostedFileBase File)
        {
            String blobURI = "";
            CloudBlobContainer blobContainer = blobService.GetCloudBlobContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(File.FileName);
            blob.UploadFromStream(File.InputStream);
            return blobURI = blob.Uri.ToString();
        }
        //Deletes a file from BLOB storage using the provided string.
        public static void deleteBlob(string fileURI)
        {
            // Retrieve reference to a previously created container.
            CloudBlobContainer blobContainer = blobService.GetCloudBlobContainer();
            //get the last part of the URI
            string toDelete = fileURI.Split('/').Last();
            // Retrieve reference to a blob named "myblob.txt".
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(toDelete);
            // Delete the blob.
            blockBlob.Delete();
        }
    }
}