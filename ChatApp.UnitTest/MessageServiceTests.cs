using Xunit;
using Moq;
using System.Threading.Tasks;
using ChatApp.Application.Services.Implementations;
using ChatApp.Application.Interfaces;
using ChatApp.Application.DTOs.Message;
using ChatApp.Domain.Entities;
using ChatApp.Application.Mappers;
using System.Collections.Generic;
using System.Linq;
using ChatApp.Domain.Exceptions;
using ChatApp.Application.Abstracts.Services;
using Microsoft.AspNetCore.Http;
using AutoFixture;
using System.Linq.Expressions;
using ChatApp.Domain.Enums;

public class MessageServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICloudinaryService> _mockCloudinaryService;
    private readonly MessageService _messageService;
    private readonly Fixture _fixture;

    public MessageServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCloudinaryService = new Mock<ICloudinaryService>();
        _messageService = new MessageService(_mockUnitOfWork.Object, _mockCloudinaryService.Object);
        _fixture = new Fixture();

        _fixture.Behaviors
       .OfType<ThrowingRecursionBehavior>()
       .ToList()
       .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task AddMessageAsync_ShouldReturnMessageDto_WithAutoFixture()
    {
        // Arrange
        var messageAddDto = _fixture.Build<MessageAddDto>()
            .With(x => x.Files, new List<IFormFile>()) // giả định không có file
            .With(x => x.Content, "Hello from AutoFixture")
            .Create();

        _mockUnitOfWork.Setup(x => x.MessageRepository.AddAsync(It.IsAny<Message>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.CompleteAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _messageService.AddMessageAsync(messageAddDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Hello from AutoFixture", result.Content);
    }

    [Fact]
    public async Task UpdateMessageAsync_ShouldReturnUpdatedMessageDto()
    {
        // Arrange
        int messageId = 1;
        var newContent = "Updated content";
        var messageEntity = _fixture.Build<Message>()
            .With(x => x.Id, messageId)
            .With(x => x.Content, "Old content")
            .Create();

        var messageUpdateDto = _fixture.Build<MessageUpdateDto>()
       .With(x => x.Id, messageId)
       .With(x => x.Content, newContent)
       .With(x => x.Status, MessageStatus.Read)
       .Create();

        _mockUnitOfWork.Setup(x => x.MessageRepository.GetAsync(It.IsAny<Expression<Func<Message, bool>>>(), It.IsAny<bool>()))
            .ReturnsAsync(messageEntity);

        _mockUnitOfWork.Setup(x => x.CompleteAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _messageService.UpdateMessageAsync(messageUpdateDto);

        // Assert
        Assert.Equal(newContent, result.Content);
    }

    [Fact]
    public async Task UpdateMessageAsync_ShouldThrowNotFound_WhenMessageNotFound()
    {
        // Arrange
        var updateDto = _fixture.Create<MessageUpdateDto>();
        _mockUnitOfWork.Setup(x => x.MessageRepository.GetAsync(It.IsAny<Expression<Func<Message, bool>>>(), It.IsAny<bool>()))
            .ReturnsAsync((Message)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _messageService.UpdateMessageAsync(updateDto));
    }

    [Fact]
    public async Task DeleteMessageAsync_ShouldReturnTrue_WhenSuccess()
    {
        // Arrange
        var message = _fixture.Build<Message>()
            .With(m => m.IsDeleted, false)
            .Create();

        _mockUnitOfWork.Setup(x => x.MessageRepository.GetAsync(It.IsAny<Expression<Func<Message, bool>>>(), It.IsAny<bool>()))
            .ReturnsAsync(message);

        _mockUnitOfWork.Setup(x => x.MessageRepository.UpdateMessageAsync(It.IsAny<Message>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(x => x.CompleteAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _messageService.DeleteMessageAsync(m => m.Id == message.Id);

        // Assert
        Assert.True(result);
    }


    [Fact]
    public async Task DeleteMessageAsync_ShouldThrowNotFound_WhenMessageNotFound()
    {
        // Arrange
        _mockUnitOfWork.Setup(x => x.MessageRepository.GetAsync(It.IsAny<Expression<Func<Message, bool>>>(), It.IsAny<bool>()))
            .ReturnsAsync((Message)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _messageService.DeleteMessageAsync(m => m.Id == 999));
    }

    //[Fact]
    //public async Task GetMessageByIdAsync_ShouldReturnDto_WhenFound()
    //{
    //    // Arrange
    //    var message = _fixture.Create<Message>();
    //    _mockUnitOfWork.Setup(x => x.MessageRepository.GetMessageByIdAsync(message.Id))
    //        .ReturnsAsync(message);

    //    // Act
    //    var result = await _messageService.GetMessageByIdAsync(message.Id);

    //    // Assert
    //    Assert.NotNull(result);
    //    Assert.Equal(message.Id, result.Id);
    //}

    //[Fact]
    //public async Task MarkMessageAsReadAsync_ShouldReturnTrue_WhenUnreadMessageExists()
    //{
    //    // Arrange
    //    var message = _fixture.Build<Message>()
    //        .With(m => m.Status, MessageStatus.Sent) // Giả định trạng thái chưa đọc
    //        .Create();

    //    _mockUnitOfWork.Setup(x => x.MessageRepository.GetUnreadMessageByIdAsync(message.Id))
    //        .ReturnsAsync(message);
    //    _mockUnitOfWork.Setup(x => x.CompleteAsync()).ReturnsAsync(true);

    //    // Act
    //    var result = await _messageService.MarkMessageAsReadAsync(message.Id);

    //    // Assert
    //    Assert.True(result);
    //    Assert.Equal(MessageStatus.Read, message.Status);
    //}


}
