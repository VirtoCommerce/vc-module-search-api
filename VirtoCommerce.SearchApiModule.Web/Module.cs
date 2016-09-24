﻿using Microsoft.Practices.Unity;
using System;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.SearchApiModule.Web.Providers.ElasticSearch.Nest;
using VirtoCommerce.SearchApiModule.Web.Providers.Lucene;
using VirtoCommerce.SearchApiModule.Web.Services;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Model.Filters;
using VirtoCommerce.SearchModule.Core.Model.Indexing;
using VirtoCommerce.SearchModule.Core.Model.Search;
using VirtoCommerce.SearchModule.Data.Providers.ElasticSearch.Nest;
using VirtoCommerce.SearchModule.Data.Providers.Lucene;

namespace VirtoCommerce.SearchApiModule.Web
{
    public class Module : ModuleBase
    {
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        #region IModule Members

        public override void Initialize()
        {
            base.Initialize();

            // register index builders
            _container.RegisterType<ISearchIndexBuilder, CatalogItemIndexBuilder>("catalogitem-indexer");
            _container.RegisterType<ISearchIndexBuilder, CategoryIndexBuilder>("category-indexer");

            _container.RegisterType<IItemBrowsingService, ItemBrowsingService>();
            _container.RegisterType<ICategoryBrowsingService, CategoryBrowsingService>();
            _container.RegisterType<IBrowseFilterService, FilterService>();
        }

        public override void PostInitialize()
        {
            base.PostInitialize();

            var searchConnection = _container.Resolve<ISearchConnection>();
            if (searchConnection.Provider.Equals(SearchProviders.Elasticsearch.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                _container.RegisterType<ISearchQueryBuilder, CatalogElasticSearchQueryBuilder>();
            }
            else if (searchConnection.Provider.Equals(SearchProviders.Lucene.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                _container.RegisterType<ISearchQueryBuilder, CatalogLuceneQueryBuilder>();
            }
        }

        #endregion
    }
    }
