namespace Calia.Services.OrderAPI.Utility
{
    public class SD
    {
		public const string StatusCreditCart = "KrediKarti";
		public const string StatusCash = "Nakit";
		public const string StatusWaiterOrder = "GarsonSiparis";
		public const string StatusShipped = "Gonderildi";
		public const string StatusPending = "Hazirlaniyor";
		public const string StatusCancelled = "IptalEdildi";
		public const string StatusRefunded = "RedEdildi";

		public const string PaymentStatusPending = "Bekliyor";
        public const string PaymentStatusIkram = "Ikram";
        public const string PaymentStatusApproved = "Onaylandi";
		public const string PaymentStatusDelayedPayment = "Garson Tarafindan Alindi Bekliyor";
		public const string PaymentStatusRejected = "RedEdildi";


		public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
		public const string RoleWaiter = "WAITER";
	}
}
