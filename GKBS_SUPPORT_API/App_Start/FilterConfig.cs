using System.Web;
using System.Web.Mvc;

namespace GKBS_SUPPORT_API
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
