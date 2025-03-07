namespace Calia.Web.Utility
{
    public class SD
    {
        public static string StockAPIBase {  get; set; }
		public static string CategoryAPIBase { get; set; }
		public static string AuthAPIBase { get; set; }
        public static string ProductAPIBase { get; set; }
        public static string ShoppingCartAPIBase { get; set; }
        public static string OrderAPIAPIBase { get; set; }

        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
		public const string RoleWaiter = "WAITER";
		public const string TokenCookie = "JWTToken";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

		public const string StatusCreditCart = "KrediKarti";
		public const string StatusCash = "Nakit";
		public const string StatusWaiterOrder = "GarsonSiparis";
		public const string StatusShipped = "Gonderildi";
		public const string StatusPending = "Hazirlaniyor";
		public const string StatusCancelled = "IptalEdildi";
		public const string StatusRefunded = "RedEdildi";
        public const string PaymentStatusIkram = "Ikram";
        public const string PaymentStatusPending = "Bekliyor";
		public const string PaymentStatusApproved = "Onaylandi";
		public const string PaymentStatusDelayedPayment = "Garson Tarafindan Alindi Bekliyor";
		public const string PaymentStatusRejected = "RedEdildi";



		public enum ContentType
        {
           Json,
           MultipartFormData
        }
    }
}
