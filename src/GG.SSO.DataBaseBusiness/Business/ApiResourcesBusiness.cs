﻿using GG.SSO.DataBaseBusiness.Sentences;
using GG.SSO.Entity.Table.Sso;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace GG.SSO.DataBaseBusiness.Business
{
    public class ApiResourcesBusiness
    {   private readonly ILogger<ApiResourcesBusiness> _logger;
        private readonly ApiResourcesSentences _apiResourcesSentences;        

        public ApiResourcesBusiness(ILogger<ApiResourcesBusiness> logger,
            ApiResourcesSentences apiResourcesSentences)
        {
            _logger = logger;
            _apiResourcesSentences = apiResourcesSentences;
        }

        public IEnumerable<ApiResources> Get()
        {
            try
            {
                return ApiResources.Read(_apiResourcesSentences.AddCliteriByIsDeleted(false).GetCriteriaCollection());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Name}", MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

    }
}
