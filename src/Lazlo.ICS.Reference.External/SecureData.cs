using Newtonsoft.Json;

namespace ThreeTwoSix.SDK.Cryptography;

public class SecureData : SecureEntityBase
{
    /// <summary>
    /// payload encrypted using decrypted SymetricKey from SymetricKeyCypherText
    /// </summary>
    [JsonProperty(PropertyName = "cypherBytes")]
    public byte[] CypherBytes { get; set; }
}

public abstract class SecureEntityBase
{
    [JsonProperty(PropertyName = "dekIV")]
    public byte[] DekIV { get; set; }

    /// <summary>
    /// data encryption key encrypted using the ics public key
    /// thus only the ics reciever can decrypt
    /// </summary>
    [JsonProperty(PropertyName = "dekKeyCypherBytes")]
    public byte[] DekKeyCypherBytes { get; set; }

    /// <summary>
    /// Digital signature cypher text
    /// </summary>
    [JsonProperty(PropertyName = "dsCypherText")]
    public string DsCypherText { get; set; }

    /// <summary>
    /// Digital signature encryption key Uri
    /// </summary>
    [JsonProperty(PropertyName = "dsekUri")]
    public string DsekUri { get; set; }

    /// <summary>
    /// The HSM Uri or Shah1 thumbprint/fingerprint that identifies the public key used to encrypt the dek
    /// </summary>
    [JsonProperty(PropertyName = "kekUri")]
    public string KekUri { get; set; }

    // like last 4 of social or last 3 of card
    [JsonProperty(PropertyName = "nonSecureHint")]
    public string NonSecureHint { get; set; }

    [JsonProperty(PropertyName = "createdOn")]
    public DateTimeOffset CreatedOn { get; set; }
}
