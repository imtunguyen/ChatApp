using AutoFixture;
using ChatApp.Application.Abstracts.Services;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Services.Implementations;
using Moq;

namespace ChatApp.UnitTest
{
    public class UserGroupServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICloudinaryService> _mockCloudinaryService;
        private readonly UserGroupService _userGroupService;
        private readonly Fixture _fixture;

        public UserGroupServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCloudinaryService = new Mock<ICloudinaryService>();
            //_userGroupService = new UserGroupService(_mockUnitOfWork.Object);
            _fixture = new Fixture();
        }
    }
}
