namespace Growflo.Integration.Core.Sage
{
    public interface ISageActionResult
    {
        string Id { get; set; }
        string Message { get; set; }
        SageActionResultType Result { get; set; }
        string Action { get; set; }
        string ToString();
    }
}