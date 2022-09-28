using Newtonsoft.Json;

namespace Lazlo.Gaming.SDK.Ruleset;

public class ComponentSelection
{
    [JsonProperty(PropertyName = "componentId")]
    public Guid ComponentId { get; set; }

    [JsonProperty(PropertyName = "optionId")]
    public Guid OptionId { get; set; }

    [JsonProperty(PropertyName = "isSystemGenerated")]
    public bool IsSystemGenerated { get; set; }
}
