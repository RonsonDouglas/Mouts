using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SalesApi.Controllers;
using SalesApi.Data.Repositories;
using SalesApi.Domain.Entities;
using Xunit;

namespace SalesApi.Tests.Unit
{
    public class VendasControllerTests
    {
        private readonly VendasController _controller;
        private readonly IVendaRepository _repositoryMock;
        private readonly ILogger<VendasController> _loggerMock;

        public VendasControllerTests()
        {
            _repositoryMock = Substitute.For<IVendaRepository>();
            _loggerMock = Substitute.For<ILogger<VendasController>>();
            _controller = new VendasController(_repositoryMock, _loggerMock);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenVendasExist()
        {
            // Arrange
            var vendas = new List<Venda> { new Venda { Id = Guid.NewGuid() } };
            _repositoryMock.ListarVendasAsync().Returns(vendas);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(vendas);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenVendaDoesNotExist()
        {
            // Arrange
            _repositoryMock.ObterVendaPorIdAsync(Arg.Any<Guid>()).Returns((Venda)null);

            // Act
            var result = await _controller.Get(Guid.NewGuid());

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WhenVendaExists()
        {
            // Arrange
            var venda = new Venda { Id = Guid.NewGuid() };
            _repositoryMock.ObterVendaPorIdAsync(venda.Id).Returns(venda);

            // Act
            var result = await _controller.Get(venda.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(venda);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenVendaIsValid()
        {
            // Arrange
            var venda = new Venda { Id = Guid.NewGuid() };

            // Act
            var result = await _controller.Create(venda);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>()
                .Which.Value.Should().BeEquivalentTo(venda);
            await _repositoryMock.Received(1).AdicionarVendaAsync(venda);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenVendaIsNull()
        {
            // Act
            var result = await _controller.Create(null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenVendaDoesNotExist()
        {
            // Arrange
            var venda = new Venda { Id = Guid.NewGuid() };
            _repositoryMock.ObterVendaPorIdAsync(venda.Id).Returns((Venda)null);

            // Act
            var result = await _controller.Update(venda.Id, venda);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenVendaIsUpdated()
        {
            // Arrange
            var venda = new Venda { Id = Guid.NewGuid() };
            _repositoryMock.ObterVendaPorIdAsync(venda.Id).Returns(venda);

            // Act
            var result = await _controller.Update(venda.Id, venda);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            await _repositoryMock.Received(1).AtualizarVendaAsync(venda);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenVendaIdDoesNotMatch()
        {
            // Arrange
            var venda = new Venda { Id = Guid.NewGuid() };

            // Act
            var result = await _controller.Update(Guid.NewGuid(), venda);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenVendaDoesNotExist()
        {
            // Arrange
            _repositoryMock.ObterVendaPorIdAsync(Arg.Any<Guid>()).Returns((Venda)null);

            // Act
            var result = await _controller.Delete(Guid.NewGuid());

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenVendaIsDeleted()
        {
            // Arrange
            var venda = new Venda { Id = Guid.NewGuid() };
            _repositoryMock.ObterVendaPorIdAsync(venda.Id).Returns(venda);

            // Act
            var result = await _controller.Delete(venda.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            await _repositoryMock.Received(1).RemoverVendaAsync(venda.Id);
        }

        [Fact]
        public async Task CancelarItem_ShouldReturnNoContent_WhenItemIsCanceled()
        {
            // Arrange
            var vendaId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var venda = new Venda
            {
                Id = vendaId,
                Itens = new List<ItemVenda>
        {
            new ItemVenda { Id = itemId }
        }
            };

            _repositoryMock.ObterVendaPorIdAsync(vendaId).Returns(venda);
            _repositoryMock.CancelarItemAsync(vendaId, itemId).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CancelarItem(vendaId, itemId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            await _repositoryMock.Received(1).CancelarItemAsync(vendaId, itemId);
        }

    }
}