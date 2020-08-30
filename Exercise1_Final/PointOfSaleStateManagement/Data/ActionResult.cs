namespace PointOfSaleStateManagement.Data
{
    public class ActionResult
    {
        public ActionResult(bool isSuccess, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccess { get; }
        public string ErrorMessage { get; }
    }
}
