using Newtonsoft.Json;

namespace Checkin.Common;

public class CheckinResponse
{ 
    public string Id { get; set; }

    public CheckinResponse(string id)
    {
        Id = id;
    }

    public string Error { get; set; }

    public bool Success { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

