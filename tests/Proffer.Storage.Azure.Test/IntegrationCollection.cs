﻿namespace Proffer.Storage.Integration.Test
{
    using Xunit;

    [CollectionDefinition(nameof(IntegrationCollection))]
    public class IntegrationCollection: ICollectionFixture<StoresFixture>
    {
    }
}
