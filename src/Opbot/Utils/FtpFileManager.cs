using System.IO;
using System.Net;
using System.Net.FtpClient;
using System.Threading.Tasks;

namespace Opbot.Utils
{
    public class FtpFileManager
    {
        private readonly string host;
        private readonly string username;
        private readonly string password;

        public FtpFileManager(string host, string username, string password)
        {
            this.host = host;
            this.username = username;
            this.password = password;
        }

        public async Task DownloadFileAsync(string fileName, string filePathToWrite)
        {
            using (FtpClient ftpClient = CreateFtpClient())
            {
                await Task.Factory.FromAsync(ftpClient.BeginConnect, ftpClient.EndConnect, state: null).ConfigureAwait(false);

                using (Stream streamToRead = await Task<Stream>.Factory.FromAsync<string>(ftpClient.BeginOpenRead, ftpClient.EndOpenRead, fileName, state: null).ConfigureAwait(false))
                using (Stream streamToWrite = File.Open(filePathToWrite, FileMode.Append))
                {
                    await streamToRead.CopyToAsync(streamToWrite).ConfigureAwait(false);
                }

            }
        }

        public async Task UploadFileAsync(string localFileName, string remotePath)
        {
            using (FtpClient ftpClient = CreateFtpClient())
            {
                await Task.Factory.FromAsync(ftpClient.BeginConnect, ftpClient.EndConnect, state: null).ConfigureAwait(false);

                using (Stream streamToRead = File.Open(localFileName, FileMode.Open))
                {
                    
                    using (Stream streamToWrite = await Task<Stream>.Factory.FromAsync<string, FtpDataType>(ftpClient.BeginOpenWrite, ftpClient.EndOpenWrite, remotePath, FtpDataType.Binary, null).ConfigureAwait(false))

                        await streamToRead.CopyToAsync(streamToWrite).ConfigureAwait(false);
                }
            }
        }

        public async Task<FtpListItem[]> ListingAsync(string remoteDirectory)
        {
            using (FtpClient ftpClient = CreateFtpClient())
            {
                await Task.Factory.FromAsync(ftpClient.BeginConnect, ftpClient.EndConnect, state: null).ConfigureAwait(false);
                var items = await Task<FtpListItem[]>.Factory.FromAsync(ftpClient.BeginGetListing, ftpClient.EndGetListing, remoteDirectory, (object)null);
                return items;
            }
        }

        // privates

        private FtpClient CreateFtpClient()
        {
            FtpClient ftpClient = new FtpClient();
            ftpClient.Host = host;
            ftpClient.Credentials = new NetworkCredential(username, password);
            ftpClient.EncryptionMode = FtpEncryptionMode.None;
            ftpClient.DataConnectionType = FtpDataConnectionType.AutoPassive;
            return ftpClient;
        }
    }
}
