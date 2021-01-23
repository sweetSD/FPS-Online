using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

/// <summary>
/// Created by sweetSD. (with Singleton)
/// 
/// AES256을 사용하는 보안 스크립트입니다.
/// _securityKey와 _securityIv를 직접 설정하여 사용해주세요.
/// 
/// </summary>
public class SDSecurityManager : SDSingleton<SDSecurityManager>
{
    private static readonly string _securityKey = "fiwkdu876au6w5uj";
    private static readonly byte[] _securityIv = new byte[] { 45, 12, 89, 47, 32, 59, 12, 48, 65, 45, 12, 50, 94, 32, 12, 54 };

    private RijndaelManaged _aes = new RijndaelManaged();

    private void Awake()
    {
        SetInstance(this, true);

        // AES 256 세팅
        _aes.KeySize = 256;
        _aes.BlockSize = 128;
        _aes.Mode = CipherMode.CBC;
        _aes.Padding = PaddingMode.PKCS7;
        _aes.Key = Encoding.UTF8.GetBytes(_securityKey);
        _aes.IV = _securityIv;
    }

    /// <summary>
    /// 주어진 데이터를 AES256으로 암호화(Encrypt) 합니다.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public string Encrypt(string data)
    {
        byte[] src = Encoding.UTF8.GetBytes(data);

        using (ICryptoTransform encrypt = _aes.CreateEncryptor(_aes.Key, _aes.IV))
        {
            byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);
            return Convert.ToBase64String(dest);
        }
    }

    /// <summary>
    /// 주어진 데이터를 AES256으로 복호화(Decrypt) 합니다.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public string Decrypt(string data)
    {
        byte[] src = Convert.FromBase64String(data);

        using (ICryptoTransform decrypt = _aes.CreateDecryptor(_aes.Key, _aes.IV))
        {
            byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
            return Encoding.UTF8.GetString(dest);
        }
    }
}
