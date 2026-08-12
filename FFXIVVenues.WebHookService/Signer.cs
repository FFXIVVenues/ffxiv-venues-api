using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.OpenSsl;
using System.Text;

namespace FFXIVVenues.WebHookService;

public class Signer
{
    private readonly Ed25519PrivateKeyParameters privateKey;

    public Signer(IConfigurationRoot config)
    {
        this.privateKey = LoadSigningKey(
            config.GetValue<string>("Signing:PrivateKeyPath", "config/private.pem")!);
    }

    public string Sign(string payload)
    {
        var bytes = Encoding.UTF8.GetBytes(payload);
        var signer = new Ed25519Signer(); // not thread-safe; one per call
        signer.Init(forSigning: true, this.privateKey);
        signer.BlockUpdate(bytes, 0, bytes.Length);
        return Convert.ToBase64String(signer.GenerateSignature());
    }

    private static Ed25519PrivateKeyParameters LoadSigningKey(string privateKeyFile)
    {
        if (!File.Exists(privateKeyFile))
            throw new FileNotFoundException("Signing private key file not found at path", privateKeyFile);

        var pem = File.ReadAllText(privateKeyFile);
        if (string.IsNullOrWhiteSpace(pem))
            throw new InvalidOperationException($"Signing private key file at '{privateKeyFile}' was empty.");

        using var reader = new StringReader(pem);
        var pemObject = new PemReader(reader).ReadObject();

        return pemObject switch
        {
            Ed25519PrivateKeyParameters key => key,
            AsymmetricCipherKeyPair pair when pair.Private is Ed25519PrivateKeyParameters key => key,
            null => throw new InvalidOperationException($"No PEM object found in '{privateKeyFile}'."),
            _ => throw new InvalidOperationException(
                $"Expected an Ed25519 private key in '{privateKeyFile}' but found {pemObject.GetType().Name}. " +
                "Generate one with: openssl genpkey -algorithm ed25519 -out private.pem")
        };
    }
}
