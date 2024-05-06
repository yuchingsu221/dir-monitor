using System.Security.Cryptography;
using System.Text;

public static class HashHelper
{
    public static string ComputeSha256Hash(string rawData)
    {
        // 使用 SHA256 創建 hash
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // 計算輸入字符串的 hash
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // 轉換為十六進制字串
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
