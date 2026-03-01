using Catalog.Application;
using Marten;

namespace Catalog.Infrastructure;

public sealed partial class CatalogService : ICatalogService
{
    private readonly IQuerySession _querySession;
    private readonly IDocumentSession _documentSession;

    public CatalogService(IQuerySession querySession, IDocumentSession documentSession)
    {
        _querySession = querySession;
        _documentSession = documentSession;
    }
}
