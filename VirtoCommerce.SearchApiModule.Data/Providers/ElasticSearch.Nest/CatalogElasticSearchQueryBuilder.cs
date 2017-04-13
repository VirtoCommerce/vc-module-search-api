﻿using System.Linq;
using Nest;
using VirtoCommerce.SearchApiModule.Data.Model;
using VirtoCommerce.SearchModule.Core.Model.Search.Criterias;
using VirtoCommerce.SearchModule.Data.Providers.ElasticSearch;

namespace VirtoCommerce.SearchApiModule.Data.Providers.ElasticSearch.Nest
{
    public class CatalogElasticSearchQueryBuilder : ElasticSearchQueryBuilder
    {
        protected override QueryContainer GetQuery<T>(ISearchCriteria criteria)
        {
            var result = base.GetQuery<T>(criteria);

            result &= GetCategoryQuery<T>(criteria as CategorySearchCriteria);
            result &= GetCatalogItemQuery<T>(criteria as CatalogItemSearchCriteria);

            return result;
        }

        protected virtual QueryContainer GetCategoryQuery<T>(CategorySearchCriteria criteria)
            where T : class
        {
            QueryContainer result = null;

            if (criteria?.Outlines != null && criteria.Outlines.Any())
            {
                result = CreateQuery("__outline", criteria.Outlines, true);
            }

            return result;
        }

        protected virtual QueryContainer GetCatalogItemQuery<T>(CatalogItemSearchCriteria criteria)
            where T : class
        {
            QueryContainer result = null;

            if (criteria != null)
            {
                result &= new DateRangeQuery { Field = "startdate", LessThanOrEqualTo = criteria.StartDate };

                if (criteria.StartDateFrom.HasValue)
                {
                    result &= new DateRangeQuery { Field = "startdate", GreaterThan = criteria.StartDateFrom.Value };
                }

                if (criteria.EndDate.HasValue)
                {
                    result &= new DateRangeQuery { Field = "enddate", GreaterThan = criteria.EndDate.Value };
                }

                if (!criteria.WithHidden)
                    result &= new TermQuery { Field = "status", Value = "visible" };

                if (criteria.Outlines != null && criteria.Outlines.Count > 0)
                {
                    result &= CreateQuery("__outline", criteria.Outlines, true);
                }

                if (!string.IsNullOrEmpty(criteria.Catalog))
                {
                    result &= CreateQuery("catalog", criteria.Catalog);
                }

                if (criteria.ClassTypes != null && criteria.ClassTypes.Count > 0)
                {
                    result &= CreateQuery("__type", criteria.ClassTypes, false);
                }
            }

            return result;
        }
    }
}
