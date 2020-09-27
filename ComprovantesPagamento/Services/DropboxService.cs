using ComprovantesPagamento.Domain.Models;
using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ComprovantesPagamento.Services
{
    public class DropboxService
    {
        private DropboxConfig _dropboxConfig;

        public DropboxService(DropboxConfig dropboxConfig)
        {
            _dropboxConfig = dropboxConfig;
        }

        private DropboxClient dropboxClient => new DropboxClient(_dropboxConfig.AccessToken);

        public async Task Upload(string path, IFormFile file)
        {
            try
            {
                var length = file.Length;

                using var fileStream = file.OpenReadStream();
                var content = new byte[length];
                fileStream.Read(content, 0, (int)length);

                using var dbx = dropboxClient;
                using var mem = new MemoryStream(content);
                var updated = await dbx.Files.UploadAsync(
                    path,
                    WriteMode.Overwrite.Instance,
                    body: mem);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DeleteResult DeletePathIfExists(string pathName)
        {
            try
            {
                var file = GetPath(pathName);
                if (file == null)
                    return null;

                using var dbx = dropboxClient;
                var result = dbx.Files.DeleteV2Async(pathName)
                    .Result;

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public Dropbox.Api.Files.Metadata GetPath(string displayPath)
        {
            try
            {
                using var dbx = dropboxClient;
                var folder = dbx.Files.ListFolderAsync(string.Empty)
                   .Result
                   .Entries
                   .FirstOrDefault(x => x.PathDisplay == displayPath);

                return folder;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Dropbox.Api.Files.Metadata CreateFolderIfNotExists(string path)
        {
            try
            {
                using var dbx = dropboxClient;
                var folder = GetPath(path);
                if (folder == null)
                    dbx.Files.CreateFolderV2Async(path);

                return GetPath(path);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
