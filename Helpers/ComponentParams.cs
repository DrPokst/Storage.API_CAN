using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.API.Helpers
{
    public class ComponentParams
    {
		public const int MaxPageSize = 50;
		public int PageNumber { get; set; } = 1;

		private int pageSize = 10;

		public int PageSize
		{
			get { return pageSize; }
			set { pageSize =  (value > MaxPageSize) ? MaxPageSize: value; }
		}

		public string Type { get; set; }
		public string Size { get; set; }
		public string Mnf { get; set; }
		public string Nominal { get; set; }
		public string BuhNr { get; set; }
		public string OrderBy { get; set; }

	}
}
