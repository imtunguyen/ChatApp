using AutoFixture;
using ChatApp.Application.Abstracts.Services;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Services.Implementations;
using Moq;

namespace ChatApp.UnitTest
{
    public class GroupServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICloudinaryService> _mockCloudinaryService;
        private readonly GroupService _groupService;
        private readonly Fixture _fixture;

        public GroupServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCloudinaryService = new Mock<ICloudinaryService>();
            //_groupService = new GroupService(_mockUnitOfWork.Object);
            _fixture = new Fixture();
        }
    }
}
