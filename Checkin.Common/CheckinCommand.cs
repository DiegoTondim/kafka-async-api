using Newtonsoft.Json;

namespace Checkin.Common;

public class CheckinCommand
{
    public string Pnr { get; set; }

    public string Passport { get; set; }

    public DateTime ExpireDate { get; set; }

    public string Nationality { get; set; }

    public string? Id { get; set; }

    public void GenerateId()
    {
        Id = Guid.NewGuid().ToString();
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

