namespace PointOfSaleStateManagement.Data
{
    public class ActionResult
    {
        public ActionResult(bool wasSuccess, string errorMessage = null)
        {
            WasSuccess = wasSuccess;
            ErrorMessage = errorMessage;
        }

        public bool WasSuccess { get; }
        public string ErrorMessage { get; }
    }
}
