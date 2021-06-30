namespace Storage.API_CAN.Helpers
{
    public class HistoryParams
    {
        
		public const int MaxPageSize = 50;
		public int PageNumber { get; set; } = 1;

		private int pageSize = 10;

		public int PageSize
		{
			get { return pageSize; }
			set { pageSize =  (value > MaxPageSize) ? MaxPageSize: value; }
		}
        public string Mnf { get; set; }
        public int OldQty { get; set; }
        public int NewQty { get; set; }
        public string OldLocation { get; set; }
        public string NewLocation { get; set; }
        public int ComponentasId { get; set; }
        public int ReelId { get; set; }
		public string OrderBy { get; set; }
    }
}