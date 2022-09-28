using Newtonsoft.Json;
using ThreeTwoSix.SDK.Cryptography;

namespace ThreeTwoSix.SDK.DataStream;

public class DataStreamEvent
{
    public DataStreamEvent()
    {

    }

    [JsonProperty(PropertyName = "correlationId")]
    public Guid CorrelationId { get; set; }

    [JsonProperty(PropertyName = "dataStreamId")]
    public Guid DataStreamId { get; set; }

    /// <summary>
    /// provies a clear text property bag for data that can inform about processing
    /// example would be the checkoutSessionId in checkout operations for actor naming
    /// </summary>
    [JsonProperty(PropertyName = "properties")]
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

    [JsonProperty(PropertyName = "createdOn")]
    public DateTimeOffset CreatedOn { get; set; }

    /// <summary>
    /// payload encrypted using decrypted SymetricKey from SymetricKeyCypherText
    /// </summary>
    [JsonProperty(PropertyName = "cypherBytesSecure")]
    public SecureData CypherBytesSecure { get; set; }
}