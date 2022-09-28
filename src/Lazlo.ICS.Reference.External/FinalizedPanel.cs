using Lazlo.Gaming.SDK.Ruleset;
using Newtonsoft.Json;
using ThreeTwoSix.SDK.Cryptography;

namespace Lazlo.Gaming.SDK.Common;

public class FinalizedPanel
{
    [JsonProperty(PropertyName = "amountPaidAtCheckout")]
    public decimal AmountPaidAtCheckout { get; set; }

    [JsonProperty(PropertyName = "authoritySelection")]
    public DrawSelection AuthoritySelection { get; set; }

    [JsonProperty(PropertyName = "brandId")]
    public Guid BrandId { get; set; } 

    [JsonProperty(PropertyName = "centralSystemSecret")]
    public SecureData CentralSystemSecret { get; set; }  

    [JsonProperty(PropertyName = "clerkId")]
    public Guid ClerkId { get; set; }

    [JsonProperty(PropertyName = "correlationId")]
    public Guid CorrelationId { get; set; } 

    [JsonProperty(PropertyName = "createdOn")]
    public DateTimeOffset CreatedOn { get; set; }

    [JsonProperty(PropertyName = "data")]
    public List<KeyValuePair<string, string>> Data { get; set; }

    [JsonProperty(PropertyName = "deletedOn")]
    public DateTimeOffset? DeletedOn { get; set; }

    [JsonProperty(PropertyName = "drawId")]
    public Guid DrawId { get; set; }

    [JsonProperty(PropertyName = "gameId")]
    public Guid GameId { get; set; }

    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; }

    [JsonProperty(PropertyName = "playerSelection")]
    public DrawSelection PlayerSelection { get; set; }

    [JsonProperty(PropertyName = "publicKey")]
    public string PublicKey { get; set; }

    [JsonProperty(PropertyName = "retailerId")]
    public Guid RetailerId { get; set; }

    [JsonProperty(PropertyName = "ticketId")]
    public Guid TicketId { get; set; } 

    [JsonProperty(PropertyName = "wagerId")]
    public Guid WagerId { get; set; } 

    [JsonProperty(PropertyName = "value")]
    public decimal Value { get; set; }

    [JsonProperty(PropertyName = "vPosId")]
    public Guid VPosId { get; set; }
}
