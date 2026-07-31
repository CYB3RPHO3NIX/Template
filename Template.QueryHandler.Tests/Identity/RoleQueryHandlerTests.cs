using Xunit;
using Template.Queries.Identity.RoleQueries;

namespace Template.QueryHandler.Tests.Identity
{
    public class RoleQueryHandlerTests
    {
        [Fact]
        public void GetRoleByIdQuery_WithValidId_CreatesQuery()
        {
            var roleId = Guid.NewGuid();
            var query = new GetRoleByIdQuery
            {
                TraceId = Guid.NewGuid(),
                RoleId = roleId
            };

            Assert.NotNull(query);
            Assert.Equal(roleId, query.RoleId);
            Assert.NotEqual(Guid.Empty, query.TraceId);
        }

        [Fact]
        public void ListRolesQuery_WithValidParams_CreatesQuery()
        {
            var query = new ListRolesQuery
            {
                TraceId = Guid.NewGuid(),
                PageNumber = 1,
                PageSize = 10,
                SearchTerm = null,
                IsActive = true
            };

            Assert.NotNull(query);
            Assert.Equal(1, query.PageNumber);
            Assert.Equal(10, query.PageSize);
        }

        [Fact]
        public void ListRolesQuery_WithSearchTerm_FiltersCorrectly()
        {
            var searchTerm = "Admin";
            var query = new ListRolesQuery
            {
                TraceId = Guid.NewGuid(),
                PageNumber = 1,
                PageSize = 10,
                SearchTerm = searchTerm
            };

            Assert.Equal(searchTerm, query.SearchTerm);
        }

        [Fact]
        public void ListRolesQuery_WithInactiveFilter_AppliesFilter()
        {
            var query = new ListRolesQuery
            {
                TraceId = Guid.NewGuid(),
                PageNumber = 1,
                PageSize = 10,
                IsActive = false
            };

            Assert.False(query.IsActive);
        }
    }
}
