using AutoFixture;
using ChatApp.Application.Abstracts.Services;
using ChatApp.Application.DTOs.FriendShip;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Mappers;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Application.Services.Implementations;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Exceptions;
using Moq;
using Xunit;

namespace ChatApp.UnitTest
{
    public class FriendShipServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICloudinaryService> _mockCloudinaryService;
        private readonly FriendShipService _friendShipService;
        private readonly Fixture _fixture;

        public FriendShipServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCloudinaryService = new Mock<ICloudinaryService>();
            _friendShipService = new FriendShipService(_mockUnitOfWork.Object);
            _fixture = new Fixture();

            _fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        }


        [Fact]
        public async Task AddFriendShip_ShouldAddNewFriendShip_WhenNoExistingRelation()
        {
            // Arrange
            var friendShipDto = _fixture.Build<FriendShipAddDto>().Create();
            var entity = FriendShipMapper.FriendShipAddDtoToEntity(friendShipDto);

            _mockUnitOfWork.Setup(u => u.FriendShipRepository.GetFriendShip(friendShipDto.RequesterId, friendShipDto.AddresseeId))
                .ReturnsAsync((ChatApp.Domain.Entities.FriendShip)null!);
            _mockUnitOfWork.Setup(u => u.FriendShipRepository.AddAsync(It.IsAny<ChatApp.Domain.Entities.FriendShip>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(true);

            // Act
            var result = await _friendShipService.AddFriendShip(friendShipDto);

            // Assert
            Assert.Equal(friendShipDto.RequesterId, result.RequesterId);
            Assert.Equal(friendShipDto.AddresseeId, result.AddresseeId);
        }



        [Fact]
        public async Task AddFriendShip_ShouldUpdateStatus_WhenAlreadyExistsWithNone()
        {
            // Arrange
            var friendShipDto = _fixture.Build<FriendShipAddDto>().Create();
            var existing = _fixture.Build<ChatApp.Domain.Entities.FriendShip>()
                .With(f => f.RequesterId, friendShipDto.RequesterId)
                .With(f => f.AddresseeId, friendShipDto.AddresseeId)
                .With(f => f.Status, FriendShipStatus.None)
                .Create();

            _mockUnitOfWork.Setup(u => u.FriendShipRepository.GetFriendShip(friendShipDto.RequesterId, friendShipDto.AddresseeId))
                .ReturnsAsync(existing);
            _mockUnitOfWork.Setup(u => u.FriendShipRepository.Update(It.IsAny<ChatApp.Domain.Entities.FriendShip>()));
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(true);

            // Act
            var result = await _friendShipService.AddFriendShip(friendShipDto);

            // Assert
            Assert.Equal(FriendShipStatus.Pending, result.Status);
        }

        [Fact]
        public async Task AddFriendShip_ShouldThrow_WhenAlreadyPending()
        {
            // Arrange
            var friendShipDto = _fixture.Build<FriendShipAddDto>().Create();
            var existing = _fixture.Build<ChatApp.Domain.Entities.FriendShip>()
                .With(f => f.RequesterId, friendShipDto.RequesterId)
                .With(f => f.AddresseeId, friendShipDto.AddresseeId)
                .With(f => f.Status, FriendShipStatus.Pending)
                .Create();

            _mockUnitOfWork.Setup(u => u.FriendShipRepository.GetFriendShip(friendShipDto.RequesterId, friendShipDto.AddresseeId))
                .ReturnsAsync(existing);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _friendShipService.AddFriendShip(friendShipDto));
        }

        [Fact]
        public async Task UpdateFriendShip_ShouldUpdate_WhenValid()
        {
            // Arrange
            var updateDto = _fixture.Build<FriendShipUpdateDto>().Create();
            var entity = FriendShipMapper.FriendShipUpdateDtoToEntity(updateDto);

            _mockUnitOfWork.Setup(u => u.FriendShipRepository.GetFriendShipId(updateDto.RequesterId, updateDto.AddresseeId))
                .ReturnsAsync(1); // giả định ID = 1
            _mockUnitOfWork.Setup(u => u.FriendShipRepository.UpdateFriendShip(It.IsAny<ChatApp.Domain.Entities.FriendShip>()));
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(true);

            // Act
            var result = await _friendShipService.UpdateFriendShip(updateDto);

            // Assert
            Assert.Equal(updateDto.Status, result.Status);
        }

        [Fact]
        public async Task UpdateFriendShip_ShouldThrow_WhenUpdateFails()
        {
            var updateDto = _fixture.Build<FriendShipUpdateDto>().Create();

            _mockUnitOfWork.Setup(u => u.FriendShipRepository.GetFriendShipId(updateDto.RequesterId, updateDto.AddresseeId))
                .ReturnsAsync(1);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _friendShipService.UpdateFriendShip(updateDto));
        }

    }

}

