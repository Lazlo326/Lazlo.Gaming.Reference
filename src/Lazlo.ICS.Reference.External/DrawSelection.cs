using Newtonsoft.Json;

namespace Lazlo.Gaming.SDK.Ruleset;

public class DrawSelection
{
    [JsonProperty(PropertyName = "componentSelections")]
    public List<ComponentSelection> ComponentSelections { get; set; } = new List<ComponentSelection>();


    [JsonProperty(PropertyName = "data")]
    public List<KeyValuePair<string, string>> Data { get; set; } = new List<KeyValuePair<string, string>>();
}
