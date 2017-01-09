using Abp.Application.Services;
using ERP.Erp.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace ERP.Erp.Interfaces
{
    public interface IElasticSearchAppService : IApplicationService
    {
        /// <summary>
        /// Index all document once only
        /// </summary>
        Task<int> IndexAll();

        /// <summary>
        /// Index current inserted document
        /// </summary>
        /// <param name="doc"></param>
        Task<IIndexResponse> IndexInserted(DocumentDto doc);

        /// <summary>
        /// ElasticSearch By name of Document
        /// </summary>
        /// <param name="name"></param>
        Task<List<DocumentDto>> SearchElasticName(string name, int pageindex, int pageSize);
    }
}
