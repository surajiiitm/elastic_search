using System;
using System.Linq;
using Nest;
using Abp.Domain.Repositories;
using ERP.ExternalServices.Interfaces;
using System.Collections.Generic;
using ERP.Users;
using ERP.Erp.dto;
using AutoMapper;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Elasticsearch.Net;

namespace ERP.Erp.Interfaces
{
    public class ElasticSearchAppService : IElasticSearchAppService
    {
        #region Constants

        public const double ElasticSearchMinScore = 0.4;

        #endregion

        #region Fields

        private readonly IRepository<Document, long> _documentRepository;
        private ElasticClient _elasticSearchClient;

        #endregion

        #region Constructors

        public ElasticSearchAppService(IRepository<Document, long> documentRepository)
        {
            _documentRepository = documentRepository;
            _elasticSearchClient = new ElasticClient(
                        new ConnectionSettings(
                            new Uri("http://localhost:9200")));
        }

        #endregion

        #region Methods

        public async Task<int> IndexAll()
        {
            var query = await _documentRepository.GetAllListAsync();
            var noOfDocumentsIndexed = 0;
            try
            {
                foreach (var query1 in query)
                {
                    var doc = new Document
                    {
                        Id = query1.Id,
                        Name = query1.Name,
                        Location = query1.Location,
                    };

                    // Index auto-created under name "documentIndex"
                    var indexResponse = await _elasticSearchClient.IndexAsync(doc, idx => idx.Index("documentindex"));
                    if(indexResponse.Created)
                    {
                        noOfDocumentsIndexed++;
                    }
                }
                return noOfDocumentsIndexed;
             }

            catch (Exception e)
            {
                return 0;
            }
        }

        public async Task<IIndexResponse> IndexInserted(DocumentDto doc)
        {
            var document = new Document
            {
                Id = doc.Id,
                Name = doc.Name,
                Location = doc.Location,
            };
            return await _elasticSearchClient
                .IndexAsync(document, idx => idx.Index("documentindex"));
        }

        public async Task<List<DocumentDto>> SearchElasticName(string name, int pageIndex = 0, int pageSize = 10)
        {
            var searchResponse = await _elasticSearchClient.SearchAsync<Document>(s => s
                .Index("documentindex")
                .From(pageIndex*pageSize)
                .Size(pageSize)
                .MinScore(ElasticSearchMinScore)
                .AnalyzeWildcard(true)
                .Query(q => q
                    .MatchPhrasePrefix(p => p
                        .Analyzer("standard")
                        .Field(f => f.Name)
                        .Query(name)
                    )
                )
                .Sort(doc => doc.Descending(SortSpecialField.Score))
            );

            return  searchResponse.Documents.Select(p => Mapper.Map<DocumentDto>(p)).ToList();
        }

        #endregion
    }
}
