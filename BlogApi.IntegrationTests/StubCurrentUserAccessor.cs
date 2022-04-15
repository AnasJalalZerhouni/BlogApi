using BlogApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.IntegrationTests
{
    internal class StubCurrentUserAccessor : ICurrentUserAccessor
    {
        private readonly int _currentUserId;
        public StubCurrentUserAccessor(int username)
        {
            _currentUserId = username;
        }

        public int? GetCurrentUserId()
        {
            return _currentUserId;
        }
    }
}
