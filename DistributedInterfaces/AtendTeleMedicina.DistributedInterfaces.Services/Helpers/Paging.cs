using System;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Helpers
{
    public static class CustomResponse
    {
        public static object CustomBody(string items, string sort, int skip, int take, int totalCount)
        {
            return new
            {
                items,
                sort,
                skip,
                take,
                totalCount
            };
        }

        public static object PagingHeader(string sort, int currentPage, int pageSize, int totalCount, int totalPages)
        {
            return new
            {
                sort,
                currentPage,
                pageSize,
                totalCount,
                totalPages
            };
        }
    }
}
