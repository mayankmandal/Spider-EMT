using DecryptDll;
using Newtonsoft.Json;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using System.Xml.Linq;

namespace Spider_EMT.Repository.Domain
{
    public class SSDataRepository: ISSDataRepository
    {
        private readonly string _ssDataFilePath;
        public SSDataRepository(string ssDataFilePath)
        {
            _ssDataFilePath = ssDataFilePath;
        }

        public async Task<IEnumerable<SSDataViewModel>> GetSsData()
        {
            var ssData = GetAllSsData();
            return ssData;
        }
        private IEnumerable<SSDataViewModel> GetAllSsData()
        {
            string encryptedJson = File.ReadAllText(_ssDataFilePath);

            DecryptText decrypt = new DecryptText();
            string decryptedJson = decrypt.Decrypt(encryptedJson);

            // Deserialize decrypted JSON string into SSDataViewModel
            IEnumerable<SSDataViewModel> AllSsData = JsonConvert.DeserializeObject<List<SSDataViewModel>>(decryptedJson);
            return AllSsData;
        }

        
    }
}
