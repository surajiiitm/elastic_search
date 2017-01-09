using Abp.Dependency;
using Abp.Threading.BackgroundWorkers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Abp.Threading.Timers;
using ERP.Erp.Interfaces;


namespace ERP.Web.BackgroundWorker
{
    public class ElasticSearchWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {

        #region Fields

        private IElasticSearchAppService _elasticSearchAppService;

        #endregion

        #region Constructors

        public ElasticSearchWorker(AbpTimer timer,
            IElasticSearchAppService elasticSearchAppService)
            : base(timer)
        {
            // 12 hour
            Timer.Period = 12 * 60 * 60 * 1000;
            _elasticSearchAppService = elasticSearchAppService;
        }

        #endregion

        #region Methods
        protected async override void DoWork()
        {
           await _elasticSearchAppService.IndexAll();
        }

        #endregion
    }
}