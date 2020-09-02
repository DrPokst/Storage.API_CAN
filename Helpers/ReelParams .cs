using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.API.Helpers
{
    public class ReelParams
    {
		public const int MaxPageSize = 28;
		public int PageNumber { get; set; } = 1;

		private int pageSize = 26;

		public int PageSize
		{
			get { return pageSize; }
			set { pageSize =  (value > MaxPageSize) ? MaxPageSize: value; }
		}

		public string CMnf { get; set; }
		public string OrderBy { get; set; }
	}
}
